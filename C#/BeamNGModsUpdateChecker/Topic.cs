using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RestSharp;

namespace BeamNGModsUpdateChecker
{
    public class Topic : IEquatable<Topic>
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public string EditMsg { get; set; }
        public DateTime EditMsgDate { get; set; }
        public bool Read { get; set; }

        #region Constructors

        public Topic() { }

        public Topic( string link, CookieContainer cookieJar )
        {
            this.Link = link;
            this.Read = true;
            this.updTitle( cookieJar );
            this.updEditMsg( cookieJar );
        }

        #endregion

        /// <summary>
        /// Converts forum date to DateTime object
        /// </summary>
        /// <param name="str">Forum date string</param>
        /// <returns></returns>
        public DateTime strToDate( string str )
        {
            DateTime dt = DateTime.Now;
            Regex regex = new Regex( "([0-9]+) (Hour(s)?|Day(s)?|Minute(s)?|Week(s)?) Ago at ([0-9]+):([0-9]+) (AM|PM)" );
            Match match = regex.Match( str );
            if ( match.Success )
            {
                double num = double.Parse( match.Groups[ 1 ].ToString() );
                string s = match.Groups[ 2 ].ToString();
                string h = int.Parse( match.Groups[ 7 ].ToString() ).ToString();
                string m = int.Parse( match.Groups[ 8 ].ToString() ).ToString();
                string ap = match.Groups[ 9 ].ToString();
                string timeS = h + ":" + m + " " + ap;
                if ( s.IndexOf( "Minute" ) >= 0 )
                {
                    dt = dt.AddMinutes( -num );
                }
                else if ( s.IndexOf( "Hour" ) >= 0 )
                {
                    dt = dt.AddHours( -num );
                    dt = dt.Date + Convert.ToDateTime( timeS ).TimeOfDay;
                }
                else if ( s.IndexOf( "Day" ) >= 0 )
                {
                    dt = dt.AddDays( -num );
                    dt = dt.Date + Convert.ToDateTime( timeS ).TimeOfDay;
                }
                else if ( s.IndexOf( "Week" ) >= 0 )
                {
                    dt = dt.AddDays( -num * 7 );
                }
                dt = dt.Date + Convert.ToDateTime( timeS ).TimeOfDay;
            }
            else
            {
                string dtS = str.Replace( "at ", "" );
                dtS = dtS.Remove( 0, dtS.IndexOf( "; " ) + 2 );
                dtS = dtS.Substring( 0, dtS.IndexOf( "." ) );
                dt = DateTime.ParseExact( dtS, "MM-dd-yyyy hh:mm tt", new DateTimeFormatInfo() );
            }
            return dt;
        }

        /// <summary>
        /// Updates thread title
        /// </summary>
        /// <param name="cookieJar">Cookies</param>
        /// <returns></returns>
        public bool updTitle( CookieContainer cookieJar )
        {
            bool result = false;
            var client = new RestClient( this.Link );
            client.CookieContainer = cookieJar;
            var request = new RestRequest( Method.GET );
            IRestResponse response = client.Execute( request );

            var h = new HtmlAgilityPack.HtmlDocument();
            h.LoadHtml( response.Content );
            HtmlNodeCollection nodes = h.DocumentNode.SelectNodes( "//title" );
            string title = nodes[ 0 ].InnerText;
            if ( title != this.Title )
            {
                result = true;
                this.Title = title;
            }
            return result;
        }

        /// <summary>
        /// Updates edit message of first post
        /// </summary>
        /// <param name="cookieJar"></param>
        /// <returns></returns>
        public bool updEditMsg( CookieContainer cookieJar )
        {
            bool result = false;
            var client = new RestClient( this.Link );
            client.CookieContainer = cookieJar;
            var request = new RestRequest( Method.GET );
            IRestResponse response = client.Execute( request );

            var h = new HtmlAgilityPack.HtmlDocument();
            h.LoadHtml( response.Content );
            var posts = h.GetElementbyId( "posts" );
            var post = posts.ChildNodes[ 1 ];
            HtmlNode postdetails = null;
            for ( int i = 0; i < post.ChildNodes.Count; i++ )
            {
                if ( post.ChildNodes[ i ].GetAttributeValue( "class", "" ) == "postdetails" )
                {
                    postdetails = post.ChildNodes[ i ];
                    break;
                }
            }
            HtmlNode postbody = null;
            for ( int i = 0; i < postdetails.ChildNodes.Count; i++ )
            {
                string lol = post.ChildNodes[ i ].GetAttributeValue( "class", "" );
                if ( postdetails.ChildNodes[ i ].GetAttributeValue( "class", "" ) == "postbody" )
                {
                    postbody = postdetails.ChildNodes[ i ];
                    break;
                }
            }
            HtmlNode after_content = null;
            for ( int i = 0; i < postbody.ChildNodes.Count; i++ )
            {
                if ( postbody.ChildNodes[ i ].GetAttributeValue( "class", "" ) == "after_content" )
                {
                    after_content = postbody.ChildNodes[ i ];
                    break;
                }
            }
            HtmlNode lastedited = null;
            for ( int i = 0; i < after_content.ChildNodes.Count; i++ )
            {
                if ( after_content.ChildNodes[ i ].GetAttributeValue( "class", "" ) == "postcontent lastedited" )
                {
                    lastedited = after_content.ChildNodes[ i ];
                    break;
                }
            }
            string editMsg = ( lastedited != null ) ? lastedited.InnerText : "";
            editMsg = editMsg.Trim( ' ', '\n', '\r', '\t' );
            if ( !string.IsNullOrEmpty( editMsg ) )
            {
                DateTime dtNew = this.strToDate( editMsg );
                DateTime dtOld = ( this.EditMsg != null ) ? this.EditMsgDate : new DateTime();
                if ( dtNew > dtOld && this.EditMsg != editMsg )
                {
                    result = true;
                    this.EditMsg = editMsg;
                    this.EditMsgDate = dtNew;
                }
            }
            return result;
        }

        public bool Equals( Topic obj )
        {
            return this.Link.Equals( obj.Link );
        }
    }
}
