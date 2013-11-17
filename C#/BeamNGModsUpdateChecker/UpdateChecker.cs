#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

#endregion

namespace BeamNGModsUpdateChecker
{
    public class UpdEventArgs : EventArgs
    {
        public Topic thread { get; set; }
    }

    public class UpdateChecker
    {
        public event EventHandler<UpdEventArgs> updEvent = delegate { };

        private List<Topic> threads;
        private string login;
        private string password;
        private string progPath;
        private CookieContainer cookieJar = new CookieContainer();
        private volatile int updProgress;
        private volatile int updMaxProgress;

        #region Additional methods

        /// <summary>
        /// MD5 hash
        /// </summary>
        /// <param name="input">Some string</param>
        /// <returns>Returns MD5 hash</returns>
        public static string GetMd5Hash( string input )
        {
            MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash( Encoding.UTF8.GetBytes( input ) );
            StringBuilder sb = new StringBuilder();
            for ( int i = 0; i < data.Length; i++ )
            {
                sb.Append( data[ i ].ToString( "x2" ) );
            }
            return sb.ToString();
        }

        public static string sendGet( string url, CookieContainer cookieJar )
        {
            try
            {
                var client = new RestClient( url );
                client.CookieContainer = cookieJar;
                var request = new RestRequest( Method.GET );
                IRestResponse response = client.Execute( request );
                return response.Content;
            }
            catch
            {
                throw new Exception( "Unable to send a request!" );
            }
        }

        #endregion Additional methods

        #region Fields

        public List<Topic> Threads
        {
            get { return this.threads; }
        }

        public int UpdProgress
        {
            get { return this.updProgress; }
        }

        public int UpdMaxProgress
        {
            get { return this.updMaxProgress; }
        }

        #endregion Fields

        public UpdateChecker( string login, string password, string progPath )
        {
            this.login = login;
            this.password = password;
            this.progPath = progPath;
            this.threads = new List<Topic>();
            try
            {
                this.loadThreads();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <returns>Returns the value indicating success of authorization</returns>
        public bool auth()
        {
            try
            {
                RestClient client = new RestClient( "http://www.beamng.com/login.php?do=login" );
                client.CookieContainer = this.cookieJar;
                RestRequest request = new RestRequest( Method.POST );
                request.AddParameter( "do", "login" );
                request.AddParameter( "url", "login.php?do=logout" );
                request.AddParameter( "vb_login_md5password", GetMd5Hash( Crypto.DecryptPassword( this.password ) ) );
                request.AddParameter( "vb_login_md5password_utf", GetMd5Hash( Crypto.DecryptPassword( this.password ) ) );
                request.AddParameter( "s", "" );
                request.AddParameter( "securitytoken", "guest" );
                request.AddParameter( "vb_login_username", Crypto.DecryptPassword( this.login ) );
                request.AddParameter( "cookieuser", "1" );
                IRestResponse response = client.Execute( request );
                string content = response.Content;
                client = null;
                request = null;
                if ( content.IndexOf( "Thank you for logging in" ) >= 0 )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw new Exception( "Unable to send a request!" );
            }
        }

        public bool auth( string login, string password )
        {
            this.login = login;
            this.password = password;
            return this.auth();
        }

        /// <summary>
        /// Do we need authorization
        /// </summary>
        /// <returns></returns>
        private bool isNeedAuth()
        {
            try
            {
                string content = UpdateChecker.sendGet( "http://www.beamng.com/forum/", this.cookieJar );
                if ( content.IndexOf( "Welcome," ) >= 0 )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }

        /// <summary>
        /// Check for updates
        /// </summary>
        /// <returns>Returns the number of updates</returns>
        public int checkUpdates()
        {
            UpdEventArgs args = new UpdEventArgs();
            this.updMaxProgress = this.threads.Count;
            this.updProgress = 0;
            if ( this.isNeedAuth() )
            {
                this.auth();
            }
            for ( int i = 0; i < this.threads.Count; i++ )
            {
                Topic thread = this.threads[ i ];
                try
                {
                    string content = UpdateChecker.sendGet( thread.Link, this.cookieJar );
                    bool titleChanged = thread.updTitle( this.cookieJar, content );
                    bool attachmentsChanged = thread.updAttachments( this.cookieJar, content );
                    args.thread = thread;
                    if ( titleChanged || attachmentsChanged )
                    {
                        thread.Read = false;
                        updEvent( this, args );
                    }
                }
                catch
                {
                }
                this.updProgress++;
                Thread.Sleep( 50 );
            }
            args = null;
            return this.getUnreadThreads();
        }

        #region Working with threads

        /// <summary>
        /// Adds thread
        /// </summary>
        /// <param name="link">Link to the thread</param>
        /// <returns>Returns the value indicating on success of addition</returns>
        public bool addThread( string link )
        {
            if ( string.IsNullOrEmpty( link ) )
            {
                return false;
            }
            if ( !link.StartsWith( "http://www.beamng.com/" ) )
            {
                return false;
            }
            if ( link.IndexOf( "?" ) >= 0 )
            {
                link = link.Substring( 0, link.IndexOf( "?" ) );
            }
            if ( link.IndexOf( "/page" ) >= 0 )
            {
                link = link.Substring( 0, link.IndexOf( "/page" ) );
            }
            if ( !this.threads.Contains( new Topic { Link = link } ) )
            {
                Topic topic = new Topic( link, this.cookieJar );
                if ( string.IsNullOrEmpty( topic.Title ) )
                {
                    return false;
                }

                this.threads.Add( topic );
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Removes thread
        /// </summary>
        /// <param name="link">Link to the thread</param>
        public void removeThread( string link )
        {
            int index = this.threads.FindIndex( p => p.Link == link );
            if ( index >= 0 )
            {
                this.threads.RemoveAt( index );
            }
        }

        /// <summary>
        /// Change read status of thread
        /// </summary>
        /// <param name="link"></param>
        /// <param name="read"></param>
        public void changeReadStatus( string link, bool read )
        {
            int index = this.threads.FindIndex( p => p.Link == link );
            if ( index >= 0 )
            {
                this.threads[ index ].Read = read;
            }
        }

        /// <summary>
        /// Unread threads
        /// </summary>
        /// <returns>Returns the value of unread threads</returns>
        public int getUnreadThreads()
        {
            List<Topic> unread = this.threads.FindAll( p => p.Read == false );
            return unread.Count;
        }

        /// <summary>
        /// Removes duplicate of threads
        /// </summary>
        public void removeDuplicates()
        {
            this.threads = this.threads.GroupBy( x => x.Link ).Select( y => y.First() ).ToList();
        }

        /// <summary>
        /// Searching threads
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <returns>Found threads</returns>
        public List<Topic> searchThreads( string keyword )
        {
            if ( string.IsNullOrEmpty( keyword ) )
            {
                return null;
            }
            CultureInfo culture = CultureInfo.CurrentCulture;
            return
                this.threads.FindAll(
                    p => culture.CompareInfo.IndexOf( p.Title, keyword, CompareOptions.IgnoreCase ) >= 0
                         || p.Link.Contains( keyword ) );
        }

        #endregion Working with threads

        #region Save/Load

        public void saveThreads()
        {
            var threads = new JSONClasses.ThreadsRoot();
            threads.Threads = this.threads;
            string s = JsonConvert.SerializeObject( threads );
            string fileName = this.progPath + @"\Threads.json";
            string fileNameBak = fileName + ".bak";
            try
            {
                if ( File.Exists( fileName ) )
                {
                    if ( File.Exists( fileNameBak ) )
                    {
                        File.Delete( fileNameBak );
                    }
                    File.Move( fileName, fileNameBak );
                }
                File.WriteAllText( fileName, s );
            }
            catch
            {
                throw new Exception( "Save error!" );
            }
            threads = null;
        }

        public void loadThreads()
        {
            string fileName = this.progPath + @"\Threads.json";
            if ( File.Exists( fileName ) )
            {
                try
                {
                    string s = File.ReadAllText( fileName );
                    var threads = JsonConvert.DeserializeObject<JSONClasses.ThreadsRoot>( s );
                    if ( threads.Threads != null )
                    {
                        this.threads = threads.Threads;
                    }
                }
                catch
                {
                    this.threads = new List<Topic>();
                    throw new Exception( "Load error!" );
                }
            }
        }

        #endregion Save/Load
    }
}