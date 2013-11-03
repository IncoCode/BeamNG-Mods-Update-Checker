using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json;
using RestSharp;

namespace BeamNGModsUpdateChecker
{
    public class UpdEventArgs : EventArgs
    {
        public Topic thread { get; set; }
    }

    public class CheckUpdEventArgs : EventArgs
    {
        public int progress { get; set; }
        public int maxProgress { get; set; }
    }

    public class UpdateChecker
    {
        List<Topic> threads;
        string login;
        string password;
        string progPath;
        CookieContainer cookieJar = new CookieContainer();
        public event EventHandler<UpdEventArgs> updEvent = delegate { };
        public event EventHandler<CheckUpdEventArgs> checkUpdEvent = delegate { };

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

        #endregion

        #region Fields

        public List<Topic> Threads
        {
            get
            {
                return this.threads;
            }
        }

        #endregion

        public UpdateChecker( string login, string password, string progPath )
        {
            this.login = login;
            this.password = password;
            this.progPath = progPath;
            this.threads = new List<Topic>();
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

        /// <summary>
        /// Check for updates
        /// </summary>
        /// <returns>Returns the number of updates</returns>
        public int checkUpdates()
        {
            UpdEventArgs args = new UpdEventArgs();
            CheckUpdEventArgs checkUpdArgs = new CheckUpdEventArgs();
            checkUpdArgs.maxProgress = this.threads.Count - 1;
            for ( int i = 0; i < this.threads.Count; i++ )
            {
                Topic thread = this.threads[ i ];
                if ( this.isNeedAuth() )
                {
                    this.auth();
                }
                string content = UpdateChecker.sendGet( thread.Link, this.cookieJar );
                bool titleChanged = thread.updTitle( this.cookieJar, content );
                bool attachmentsChanged = thread.updAttachments( this.cookieJar, content );
                args.thread = thread;
                if ( titleChanged || attachmentsChanged )
                {
                    thread.Read = false;
                    updEvent( this, args );
                }
                checkUpdArgs.progress = i;
                checkUpdEvent( this, checkUpdArgs );
                Thread.Sleep( 50 );
            }
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
            if ( !link.StartsWith( "http://" ) )
            {
                link = "http://" + link;
            }
            if ( link.IndexOf( "?" ) >= 0 )
            {
                link = link.Substring( 0, link.IndexOf( "?" ) );
            }
            if ( link.IndexOf( "/page" ) >= 0 )
            {
                link = link.Substring( 0, link.IndexOf( "/page" ) );
            }
            if ( !link.StartsWith( "http://www.beamng.com/" ) )
            {
                return false;
            }
            if ( !this.threads.Contains( new Topic { Link = link } ) )
            {
                this.threads.Add( new Topic( link, this.cookieJar ) );
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
            return this.threads.FindAll( p => culture.CompareInfo.IndexOf( p.Title, keyword, CompareOptions.IgnoreCase ) >= 0
                || p.Link.Contains( keyword ) );
        }

        #endregion

        #region Save/Load

        public void saveThreads()
        {
            var threads = new JSONClasses.ThreadsRoot();
            threads.Threads = this.threads;
            string s = JsonConvert.SerializeObject( threads );
            string fileName = this.progPath + @"\Threads.json";
            string fileNameBak = fileName + ".bak";
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

        public void loadThreads()
        {
            string fileName = this.progPath + @"\Threads.json";
            if ( File.Exists( fileName ) )
            {
                string s = File.ReadAllText( fileName );
                var threads = JsonConvert.DeserializeObject<JSONClasses.ThreadsRoot>( s );
                if ( threads.Threads != null )
                {
                    this.threads = threads.Threads;
                }
            }
        }

        #endregion
    }
}
