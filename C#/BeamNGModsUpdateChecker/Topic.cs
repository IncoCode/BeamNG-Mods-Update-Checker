#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RestSharp;

#endregion

namespace BeamNGModsUpdateChecker
{
    public class Topic : IEquatable<Topic>
    {
        public string Link { get; set; }
        public string Title { get; set; }
        public List<Attachment> Attachments { get; set; }
        public bool Read { get; set; }

        #region Constructors

        public Topic()
        {
        }

        public Topic( string link, CookieContainer cookieJar )
        {
            this.Link = link;
            this.Read = true;
            this.Attachments = new List<Attachment>();

            var client = new RestClient( this.Link );
            client.CookieContainer = cookieJar;
            var request = new RestRequest( Method.GET );
            IRestResponse response = client.Execute( request );
            this.UpdTitle( response.Content );
            this.UpdAttachments( response.Content );
        }

        #endregion

        /// <summary>
        /// Updates thread title
        /// </summary>
        /// <param name="content">HTML</param>
        /// <returns></returns>
        public bool UpdTitle( string content )
        {
            if ( this.Title != null )
            {
                this.Title = this.NormalTitle( this.Title );
            }
            bool result = false;
            var h = new HtmlAgilityPack.HtmlDocument();
            h.LoadHtml( content );
            HtmlNodeCollection nodes = h.DocumentNode.SelectNodes( "//title" );
            if ( nodes == null )
            {
                return false;
            }

            string title = this.NormalTitle( nodes[ 0 ].InnerText );
            if ( title != this.Title )
            {
                result = true;
                this.Title = title;
            }
            return result;
        }

        private string NormalTitle( string str )
        {
            string result = str;
            string[] prefixs =
            {
                @"\[WIP Beta released\]",
                @"\[WIP\]",
                @"\[On Hold\]",
                @"\[Released\]",
                @"\[Cancelled\]",
                @"\[Old and Unsupported\]"
            };
            var regex = new Regex( "(" + string.Join( "|", prefixs ) + ")" );
            Match match = regex.Match( str );
            if ( !match.Success )
            {
                regex = new Regex( "(" + string.Join( "|", prefixs ).Replace( "\\[", "" ).Replace( "\\]", "" ) + ")" );
                match = regex.Match( str );
                if ( match.Success )
                {
                    string findTag = match.Groups[ 1 ].ToString();
                    result = str.Replace( findTag, "[" + findTag + "]" );
                }
            }
            return WebUtility.HtmlDecode( result );
        }

        /// <summary>
        /// Updates first post attachments
        /// </summary>
        /// <param name="content">HTML</param>
        /// <returns></returns>
        public bool UpdAttachments( string content )
        {
            bool result = false;
            var h = new HtmlAgilityPack.HtmlDocument();
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
            if ( postdetails == null )
            {
                return false;
            }
            for ( int i = 0; i < postdetails.ChildNodes.Count; i++ )
            {
                if ( postdetails.ChildNodes[ i ].GetAttributeValue( "class", "" ) == "postbody" )
                {
                    postbody = postdetails.ChildNodes[ i ];
                    break;
                }
            }
            if ( postbody == null )
            {
                return false;
            }
            HtmlNodeCollection l =
                postbody.SelectNodes(
                    "//div[@class][1]//div[@class][2]//div[@id][5]//ol[@id][1]//li[@id][1]//div[@class][2]//div[@class][1]//div[@class][1]//div[@class][1]" );
            HtmlNode attachments = null;
            if ( l == null )
            {
                l =
                    postbody.SelectNodes(
                        "//div[@class][1]//div[@class][2]//div[@id][6]//ol[@id][1]//li[@id][1]//div[@class][2]//div[@class][1]//div[@class][1]//div[@class][1]" );
            }
            if ( l == null )
            {
                return false;
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
                var regex = new Regex( "\"postcontent\"" );
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
                    if ( files == null )
                    {
                        return false;
                    }
                    var attachmentsList = new List<Attachment>();
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

        #region Ovverides

        public bool Equals( Topic t )
        {
            if ( t == null )
            {
                return false;
            }
            var regex = new Regex( @"(http://(www\.)?beamng\.com/threads/[0-9]+)" );
            string link = regex.Match( this.Link ).Groups[ 1 ].ToString();
            string link2 = regex.Match( t.Link ).Groups[ 1 ].ToString();
            return link.Equals( link2 );
        }

        public override bool Equals( object obj )
        {
            if ( obj == null )
            {
                return false;
            }
            var t = obj as Topic;
            // if not a Topic
            if ( t == null )
            {
                return false;
            }
            return this.Equals( t );
        }

        public override int GetHashCode()
        {
            var regex = new Regex( @"(http://(www\.)?beamng\.com/threads/[0-9]+)" );
            string link = regex.Match( this.Link ).Groups[ 1 ].ToString();
            return link.GetHashCode();
        }

        #endregion
    }
}