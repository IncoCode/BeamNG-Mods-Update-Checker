using System;
using System.Collections.Generic;
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

    public class UpdateChecker
    {
        List<Topic> threads;
        string login;
        string password;
        string progPath;
        CookieContainer cookieJar = new CookieContainer();
        public event EventHandler<UpdEventArgs> updEvent = delegate { };

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
            this.auth();
        }

        /// <summary>
        /// Authorization
        /// </summary>
        /// <returns>Returns the value indicating success of authorization</returns>
        public bool auth()
        {
            var client = new RestClient( "http://www.beamng.com/login.php?do=login" );
            client.CookieContainer = this.cookieJar;
            var request = new RestRequest( Method.POST );
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

        /// <summary>
        /// Do we need authorization
        /// </summary>
        /// <returns></returns>
        private bool isNeedAuth()
        {
            var client = new RestClient( "http://www.beamng.com/forum/" );
            client.CookieContainer = this.cookieJar;
            var request = new RestRequest( Method.GET );
            IRestResponse response = client.Execute( request );
            string content = response.Content;
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
            int updatesCount = 0;
            for ( int i = 0; i < this.threads.Count; i++ )
            {
                Topic thread = this.threads[ i ];
                if ( this.isNeedAuth() )
                {
                    this.auth();
                }
                bool titleChanged = thread.updTitle( this.cookieJar );
                bool editMsgChanged = thread.updEditMsg( this.cookieJar );
                args.thread = thread;
                if ( titleChanged || editMsgChanged )
                {
                    updatesCount++;
                    thread.Read = false;
                    updEvent( this, args );
                }
                Thread.Sleep( 50 );
            }
            return updatesCount;
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
        /// Marks thread as read
        /// </summary>
        /// <param name="link">Link to the thread</param>
        public void makeRead( string link )
        {
            int index = this.threads.FindIndex( p => p.Link == link );
            if ( index >= 0 )
            {
                this.threads[ index ].Read = true;
            }
        }

        #endregion

        #region Save/Load

        public void saveThreads()
        {
            var threads = new JSONClasses.ThreadsRoot();
            threads.Threads = this.threads;
            string s = JsonConvert.SerializeObject( threads );
            File.WriteAllText( this.progPath + @"\Threads.json", s );
        }

        public void loadThreads()
        {
            if ( File.Exists( this.progPath + @"\Threads.json" ) )
            {
                string s = File.ReadAllText( this.progPath + @"\Threads.json" );
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
