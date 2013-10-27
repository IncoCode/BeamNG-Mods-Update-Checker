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

        public DateTime strToDate( string str )
        {
            DateTime dt = DateTime.Now;
            Regex regex = new Regex( "([0-9]+) (Hour(s)?|Day(s)?|Minute(s)?) Ago" );
            Match match = regex.Match( str );
            if ( match.Success )
            {
                if ( match.Groups[ 2 ].ToString().IndexOf( "Hour" ) >= 0 )
                {
                    dt = dt.AddHours( -double.Parse( match.Groups[ 1 ].ToString() ) );
                }
                else if ( match.Groups[ 2 ].ToString().IndexOf( "Day" ) >= 0 )
                {
                    dt = dt.AddDays( -double.Parse( match.Groups[ 1 ].ToString() ) );
                }
                else if ( match.Groups[ 2 ].ToString().IndexOf( "Minute" ) >= 0 )
                {
                    dt = dt.AddMinutes( -double.Parse( match.Groups[ 1 ].ToString() ) );
                }
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
                DateTime dtOld = ( this.EditMsg != null ) ? this.strToDate( this.EditMsg ) : new DateTime();
                if ( dtNew > dtOld )
                {
                    result = true;
                    this.EditMsg = editMsg;
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
