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
            string content = response.Content;
            this.UpdTitle( content );
            this.UpdAttachments( content );
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
            var h = new HtmlDocument();
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
            var h = new HtmlDocument();
            h.LoadHtml( content );
            HtmlNode posts = h.GetElementbyId( "posts" );
            if ( posts == null )
            {
                return false;
            }

            var attachmentsList = new List<Attachment>();
            HtmlNodeCollection l =
                posts.SelectNodes(
                    "//div[@class][1]//div[@class][2]//div[@id][5]//ol[@id][1]//li[@id][1]//div[@class][2]//div[@class][1]//div[@class][1]//div[@class][1]" );
            HtmlNode attachments = null;
            if ( l == null )
            {
                l =
                    posts.SelectNodes(
                        "//div[@class][1]//div[@class][2]//div[@id][6]//ol[@id][1]//li[@id][1]//div[@class][2]//div[@class][1]//div[@class][1]//div[@class][1]" );
            }
            if ( l != null )
            {
                foreach ( HtmlNode node in l )
                {
                    if ( node.InnerHtml.IndexOf( "Attached Files", StringComparison.Ordinal ) >= 0 )
                    {
                        attachments = node;
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
                        foreach ( HtmlNode node in attachments.ChildNodes )
                        {
                            if ( node.GetAttributeValue( "class", "" ) == "postcontent" )
                            {
                                count++;
                            }
                            if ( count > 1 )
                            {
                                attachment = node;
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
                        foreach ( HtmlNode t in attachment.ChildNodes )
                        {
                            if ( t.Name == "ul" )
                            {
                                files = t;
                                break;
                            }
                        }
                        if ( files != null )
                        {
                            foreach ( HtmlNode file in files.ChildNodes )
                            {
                                for ( int j = 0; j < file.ChildNodes.Count; j++ )
                                {
                                    if ( file.ChildNodes[ j ].Name == "a" )
                                    {
                                        string name = file.ChildNodes[ j ].InnerText;
                                        string s = file.InnerText;
                                        string size = s.Substring( s.IndexOf( "(", StringComparison.Ordinal ) + 1,
                                            s.IndexOf( ",", StringComparison.Ordinal ) - 1 -
                                            s.IndexOf( "(", StringComparison.Ordinal ) );
                                        attachmentsList.Add( new Attachment( name, size ) );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #region Getting attachments with random link name

            string firstPostHTML = posts.ChildNodes[ 1 ].InnerHtml; // getting first post html
            var attReg =
                new Regex(
                    "<a\\shref=\"http://www\\.beamng\\.com/attachment\\.php\\?attachmentid=[^\"]*\"\\s+title=\"Name:\\s+(.*)\\s+Views:\\s[0-9]+\\s+Size:\\s+([^\"]*)\">([^<>]*)</a>" );
            MatchCollection attRegCollection = attReg.Matches( firstPostHTML );
            if ( attRegCollection.Count != 0 )
            {
                attachmentsList.AddRange( ( from Match attMatch in attRegCollection
                    select new Attachment( attMatch.Groups[ 3 ].ToString(), attMatch.Groups[ 2 ].ToString() ) ).ToList() );
            }

            #endregion

            attachmentsList = attachmentsList.Distinct().ToList();
            if ( this.Attachments == null )
            {
                this.Attachments = new List<Attachment>();
            }
            if ( !attachmentsList.SequenceEqual( this.Attachments, new Attachment() ) )
            {
                result = true;
                this.Attachments = attachmentsList;
            }
            return result;
        }

        #region Members

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
            var t = obj as Topic;
            // if not a Topic
            return t != null && this.Equals( t );
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