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
        public Topic Thread { get; set; }
    }

    public class UpdateChecker
    {
        public event EventHandler<UpdEventArgs> UpdEvent = delegate { };

        private List<Topic> _threads;
        private string _login;
        private string _password;
        private readonly string _progPath;
        private readonly CookieContainer _cookieJar = new CookieContainer();
        private volatile int _updProgress;
        private volatile int _updMaxProgress;

        #region Additional methods

        /// <summary>
        /// MD5 hash
        /// </summary>
        /// <param name="input">Some string</param>
        /// <returns>Returns MD5 hash</returns>
        public static string GetMd5Hash( string input )
        {
            var md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash( Encoding.UTF8.GetBytes( input ) );
            var sb = new StringBuilder();
            for ( int i = 0; i < data.Length; i++ )
            {
                sb.Append( data[ i ].ToString( "x2" ) );
            }
            return sb.ToString();
        }

        public static string SendGet( string url, CookieContainer cookieJar )
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
            get { return this._threads; }
        }

        public int UpdProgress
        {
            get { return this._updProgress; }
        }

        public int UpdMaxProgress
        {
            get { return this._updMaxProgress; }
        }

        #endregion Fields

        public UpdateChecker( string login, string password, string progPath )
        {
            this._login = login;
            this._password = password;
            this._progPath = progPath;
            this._threads = new List<Topic>();
            try
            {
                this.LoadThreads();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <returns>Returns the value indicating success of authorization</returns>
        public bool Auth()
        {
            try
            {
                var client = new RestClient( "http://www.beamng.com/login.php?do=login" );
                client.CookieContainer = this._cookieJar;
                RestRequest request = new RestRequest( Method.POST );
                request.AddParameter( "do", "login" );
                request.AddParameter( "url", "login.php?do=logout" );
                request.AddParameter( "vb_login_md5password", GetMd5Hash( Crypto.DecryptPassword( this._password ) ) );
                request.AddParameter( "vb_login_md5password_utf", GetMd5Hash( Crypto.DecryptPassword( this._password ) ) );
                request.AddParameter( "s", "" );
                request.AddParameter( "securitytoken", "guest" );
                request.AddParameter( "vb_login_username", Crypto.DecryptPassword( this._login ) );
                request.AddParameter( "cookieuser", "1" );
                IRestResponse response = client.Execute( request );
                string content = response.Content;
                if ( content.IndexOf( "Thank you for logging in" ) >= 0 )
                {
                    return true;
                }
                return false;
            }
            catch
            {
                throw new Exception( "Unable to send a request!" );
            }
        }

        public bool Auth( string login, string password )
        {
            this._login = login;
            this._password = password;
            return this.Auth();
        }

        /// <summary>
        /// Do we need authorization
        /// </summary>
        /// <returns></returns>
        private bool IsNeedAuth()
        {
            try
            {
                string content = UpdateChecker.SendGet( "http://www.beamng.com/forum/", this._cookieJar );
                if ( content.IndexOf( "Welcome," ) >= 0 )
                {
                    return false;
                }
                return true;
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
        public int CheckUpdates()
        {
            var args = new UpdEventArgs();
            this._updMaxProgress = this._threads.Count;
            this._updProgress = 0;
            if ( this.IsNeedAuth() )
            {
                this.Auth();
            }
            for ( int i = 0; i < this._threads.Count; i++ )
            {
                Topic thread = this._threads[ i ];
                try
                {
                    string content = UpdateChecker.SendGet( thread.Link, this._cookieJar );
                    bool titleChanged = thread.UpdTitle( content );
                    bool attachmentsChanged = thread.UpdAttachments( content );
                    args.Thread = thread;
                    if ( titleChanged || attachmentsChanged )
                    {
                        thread.Read = false;
                        this.UpdEvent( this, args );
                    }
                }
                catch
                {
                }
                this._updProgress++;
                Thread.Sleep( 30 );
            }
            return this.GetUnreadThreads();
        }

        #region Working with threads

        /// <summary>
        /// Adds thread
        /// </summary>
        /// <param name="link">Link to the thread</param>
        /// <returns>Returns the value indicating on success of addition</returns>
        public bool AddThread( string link )
        {
            if ( string.IsNullOrEmpty( link ) )
            {
                return false;
            }
            if ( !link.StartsWith( "http://" ) )
            {
                link = "http://" + link;
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
            if ( !this._threads.Contains( new Topic { Link = link } ) )
            {
                var topic = new Topic( link, this._cookieJar );
                if ( string.IsNullOrEmpty( topic.Title ) )
                {
                    return false;
                }

                this._threads.Add( topic );
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes thread
        /// </summary>
        /// <param name="link">Link to the thread</param>
        public void RemoveThread( string link )
        {
            int index = this._threads.FindIndex( p => p.Link == link );
            if ( index >= 0 )
            {
                this._threads.RemoveAt( index );
            }
        }

        /// <summary>
        /// Change read status of thread
        /// </summary>
        /// <param name="link"></param>
        /// <param name="read"></param>
        public void ChangeReadStatus( string link, bool read )
        {
            int index = this._threads.FindIndex( p => p.Link == link );
            if ( index >= 0 )
            {
                this._threads[ index ].Read = read;
            }
        }

        /// <summary>
        /// Unread threads
        /// </summary>
        /// <returns>Returns the value of unread threads</returns>
        public int GetUnreadThreads()
        {
            List<Topic> unread = this._threads.FindAll( p => !p.Read );
            return unread.Count;
        }

        /// <summary>
        /// Only unread (updated) threads
        /// </summary>
        /// <returns></returns>
        public List<Topic> GetOnlyUpdatedThreads()
        {
            return this._threads.FindAll( p => !p.Read );
        }

        /// <summary>
        /// Removes duplicate of threads
        /// </summary>
        public void RemoveDuplicates()
        {
            this._threads = this._threads.Distinct().ToList();
        }

        /// <summary>
        /// Searching threads
        /// </summary>
        /// <param name="keyword">Search keyword</param>
        /// <returns>Found threads</returns>
        public List<Topic> SearchThreads( string keyword )
        {
            if ( string.IsNullOrEmpty( keyword ) )
            {
                return null;
            }
            CultureInfo culture = CultureInfo.CurrentCulture;
            return
                this._threads.FindAll(
                    p => culture.CompareInfo.IndexOf( p.Title, keyword, CompareOptions.IgnoreCase ) >= 0
                         || p.Link.Contains( keyword ) );
        }

        #endregion Working with threads

        #region Save/Load

        public void SaveThreads()
        {
            var threads = new JSONClasses.ThreadsRoot();
            threads.Threads = this._threads;
            string s = JsonConvert.SerializeObject( threads );
            string fileName = this._progPath + @"\Threads.json";
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
        }

        public void LoadThreads()
        {
            string fileName = this._progPath + @"\Threads.json";
            if ( File.Exists( fileName ) )
            {
                try
                {
                    string s = File.ReadAllText( fileName );
                    var threads = JsonConvert.DeserializeObject<JSONClasses.ThreadsRoot>( s );
                    if ( threads.Threads != null )
                    {
                        this._threads = threads.Threads;
                    }
                }
                catch
                {
                    this._threads = new List<Topic>();
                    throw new Exception( "Load error!" );
                }
            }
        }

        #endregion Save/Load
    }
}