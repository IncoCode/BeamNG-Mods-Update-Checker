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
        public List<Attachment> Attachments { get; set; }
        public bool Read { get; set; }

        #region Constructors

        public Topic() { }

        public Topic( string link, CookieContainer cookieJar )
        {
            this.Link = link;
            this.Read = true;
            this.Attachments = new List<Attachment>();

            var client = new RestClient( this.Link );
            client.CookieContainer = cookieJar;
            var request = new RestRequest( Method.GET );
            IRestResponse response = client.Execute( request );
            this.updTitle( cookieJar, response.Content );
            this.updAttachments( cookieJar, response.Content );
        }

        #endregion

        /// <summary>
        /// Updates thread title
        /// </summary>
        /// <param name="cookieJar">Cookies</param>
        /// <param name="content">HTML</param>
        /// <returns></returns>
        public bool updTitle( CookieContainer cookieJar, string content )
        {
            bool result = false;
            var h = new HtmlAgilityPack.HtmlDocument();
            h.LoadHtml( content );
            HtmlNodeCollection nodes = h.DocumentNode.SelectNodes( "//title" );
            if ( nodes == null )
            {
                return result;
            }

            string title = nodes[ 0 ].InnerText;
            if ( title != this.Title )
            {
                result = true;
                this.Title = title;
            }
            return result;
        }

        /// <summary>
        /// Updates first post attachments
        /// </summary>
        /// <param name="cookieJar">Cookies</param>
        /// <param name="content">HTML</param>
        /// <returns></returns>
        public bool updAttachments( CookieContainer cookieJar, string content )
        {
            bool result = false;
            HtmlDocument h = new HtmlAgilityPack.HtmlDocument();
            h.LoadHtml( content );
            HtmlNode posts = h.GetElementbyId( "posts" );
            if ( posts == null )
            {
                return false;
            }

            HtmlNode post = posts.ChildNodes[ 1 ];
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
            HtmlNodeCollection l = postbody.SelectNodes( "//div[@class][1]//div[@class][2]//div[@id][5]//ol[@id][1]//li[@id][1]//div[@class][2]//div[@class][1]//div[@class][1]//div[@class][1]" );
            HtmlNode attachments = null;
            if ( l == null )
            {
                return result;
            }
            for ( int i = 0; i < l.Count; i++ )
            {
                if ( l[ i ].InnerHtml.IndexOf( "Attached Files" ) >= 0 )
                {
                    attachments = l[ i ];
                    break;
                }
            }
            HtmlNode attachment = null;
            if ( attachments != null )
            {
                Regex regex = new Regex( "\"postcontent\"" );
                MatchCollection match = regex.Matches( attachments.InnerHtml );
                if ( match.Count > 1 )
                {
                    int count = 0;
                    for ( int i = 0; i < attachments.ChildNodes.Count; i++ )
                    {
                        if ( attachments.ChildNodes[ i ].GetAttributeValue( "class", "" ) == "postcontent" )
                        {
                            count++;
                        }
                        if ( count > 1 )
                        {
                            attachment = attachments.ChildNodes[ i ];
                            break;
                        }
                    }
                }
                else
                {
                    attachment = attachments.ChildNodes[ 1 ];
                }
                HtmlNode files = null;
                if ( attachment != null )
                {
                    for ( int i = 0; i < attachment.ChildNodes.Count; i++ )
                    {
                        if ( attachment.ChildNodes[ i ].Name == "ul" )
                        {
                            files = attachment.ChildNodes[ i ];
                            break;
                        }
                    }
                    List<Attachment> attachmentsList = new List<Attachment>();
                    for ( int i = 0; i < files.ChildNodes.Count; i++ )
                    {
                        HtmlNode file = files.ChildNodes[ i ];
                        for ( int j = 0; j < file.ChildNodes.Count; j++ )
                        {
                            if ( file.ChildNodes[ j ].Name == "a" )
                            {
                                string name = file.ChildNodes[ j ].InnerText;
                                string s = file.InnerText;
                                string size = s.Substring( s.IndexOf( "(" ) + 1, s.IndexOf( "," ) - 1 - s.IndexOf( "(" ) );
                                attachmentsList.Add( new Attachment( name, size ) );
                            }
                        }
                    }
                    if ( this.Attachments == null )
                    {
                        this.Attachments = new List<Attachment>();
                    }
                    if ( !attachmentsList.SequenceEqual( this.Attachments, new Attachment() ) )
                    {
                        result = true;
                        this.Attachments = attachmentsList;
                    }
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
