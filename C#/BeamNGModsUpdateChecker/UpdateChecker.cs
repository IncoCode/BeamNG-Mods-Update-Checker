#region Using

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

#endregion

namespace BeamNGModsUpdateChecker
{

    #region Add. class

    public class ThreadFilter
    {
        public bool ShowOnlyUnread = false;
        public string SearchKeyword = "";
    }

    #endregion

    public class UpdateChecker
    {
        public readonly ThreadFilter ThreadFilter;
        private const int MaxCheckUpdCount = 3;

        private List<Topic> _threads;
        private readonly string _progPath;
        private readonly CookieContainer _cookieJar = new CookieContainer();
        private volatile int _updProgress;
        private volatile int _updMaxProgress;

        #region Additional methods

        private static string SendGet( string url, CookieContainer cookieJar )
        {
            try
            {
                var client = new RestClient( url ) { CookieContainer = cookieJar };
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
            get
            {
                List<Topic> threads = this._threads;
                if ( this.ThreadFilter.ShowOnlyUnread )
                {
                    threads = this._threads.FindAll( p => !p.Read );
                }
                string keyword = this.ThreadFilter.SearchKeyword;
                if ( !string.IsNullOrEmpty( keyword ) )
                {
                    CultureInfo culture = CultureInfo.CurrentCulture;
                    threads = threads.FindAll(
                        p => culture.CompareInfo.IndexOf( p.Title, keyword, CompareOptions.IgnoreCase ) >= 0
                             || p.Link.Contains( keyword ) );
                }
                return threads;
            }
        }

        public List<Topic> UnreadThreads
        {
            get { return this._threads.FindAll( p => !p.Read ); }
        }

        public int UnreadThreadsCount
        {
            get { return this.UnreadThreads.Count; }
        }

        public int UpdProgress
        {
            get { return this._updProgress; }
        }

        public int UpdMaxProgress
        {
            get { return this._updMaxProgress; }
        }

        public Topic this[ int index ]
        {
            get
            {
                if ( index > this._threads.Count - 1 )
                {
                    return null;
                }
                return this._threads[ index ];
            }
        }

        #endregion Fields

        public UpdateChecker()
        {
            this._progPath = Directory.GetParent( Path.GetDirectoryName( ConfigurationManager.OpenExeConfiguration( ConfigurationUserLevel.PerUserRoamingAndLocal ).FilePath ) ).FullName;
            this._threads = new List<Topic>();
            this.ThreadFilter = new ThreadFilter();
            try
            {
                this.LoadThreads();
            }
            catch
            {
            }
        }

        private int GetMaxCheckUpdCount( int index, int maxCheckUpdCount )
        {
            int newMaxCheckUpdCount = maxCheckUpdCount;
            if ( index > this._threads.Count - 1 )
            {
                return 0;
            }
            while ( index + newMaxCheckUpdCount > this._threads.Count - 1 )
            {
                newMaxCheckUpdCount--;
            }
            if ( newMaxCheckUpdCount == 0 )
            {
                return 1;
            }
            return newMaxCheckUpdCount;
        }

        /// <summary>
        /// Check for updates (one thread)
        /// </summary>
        /// <param name="thread">Thread object</param>
        /// <returns></returns>
        private async Task CheckTopicUpdate( Topic thread )
        {
            await TaskEx.Run( () =>
            {
                if ( thread == null )
                {
                    return;
                }
                try
                {
                    string content = SendGet( thread.Link, this._cookieJar );
                    bool titleChanged = thread.UpdTitle( content );
                    bool attachmentsChanged = thread.UpdAttachments( content );
                    if ( titleChanged || attachmentsChanged )
                    {
                        thread.Read = false;
                    }
                }
                catch
                {
                }
                this._updProgress++;
            } );
        }

        /// <summary>
        /// Check for updates
        /// </summary>
        /// <returns>Returns the number of updates</returns>
        public int CheckUpdates()
        {
            this._updMaxProgress = this._threads.Count;
            this._updProgress = 0;
            int i = 0;
            while ( i < this._threads.Count )
            {
                int taskArrSize = this.GetMaxCheckUpdCount( i, MaxCheckUpdCount );
                Task[] tasks = new Task[ taskArrSize ];
                for ( int j = 0; j < taskArrSize; j++ )
                {
                    tasks[ j ] = this.CheckTopicUpdate( this[ j + i ] );
                }
                Task.WaitAll( tasks );
                i += taskArrSize;
                Thread.Sleep( 30 );
            }
            return this.UnreadThreadsCount;
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
            if ( link.IndexOf( "?", StringComparison.Ordinal ) >= 0 )
            {
                link = link.Substring( 0, link.IndexOf( "?", StringComparison.Ordinal ) );
            }
            if ( link.IndexOf( "/page", StringComparison.Ordinal ) >= 0 )
            {
                link = link.Substring( 0, link.IndexOf( "/page", StringComparison.Ordinal ) );
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
        /// Removes duplicate of threads
        /// </summary>
        public void RemoveDuplicates()
        {
            this._threads = this._threads.Distinct().ToList();
        }

        #endregion Working with threads

        #region Save/Load

        public void SaveThreads()
        {
            var threads = new JSONClasses.ThreadsRoot { Threads = this._threads };
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
                if ( !Directory.Exists( this._progPath ) )
                {
                    Directory.CreateDirectory( this._progPath );
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