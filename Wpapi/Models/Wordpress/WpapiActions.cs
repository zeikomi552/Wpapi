using Markdig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL.Models;
using Wpapi.Models.Config;

namespace Wpapi.Models.Wordpress
{
    public class WpapiActions : WpapiBase
    {
        #region アクション一覧
        /// <summary>
        /// アクション一覧
        /// </summary>
        public static List<WpapiAction> Actions = new List<WpapiAction>()
        {
            new WpapiAction("/?", "ヘルプを表示します",
                Help),
            new WpapiAction("/h", "ヘルプを表示します\r\n",
                Help),
            new WpapiAction("/regist", "各種キーの保存処理" + "\r\n"
                + "\t\t-u WordPressユーザーID(必須)" + "\r\n"
                + "\t\t-p WordPressパスワード(必須)" + "\r\n"
                + "\t\t-keysfile キーファイルの保存先(省略時はデフォルトのパス)" + "\r\n"
                , Regist),
            new WpapiAction("/backno", "バックナンバー関連コマンド" + "\r\n"
                + "\t\t-type showもしくはpost showバックナンバー一覧を取得します postバックナンバー記事を固定ページに投稿します" + "\r\n"
                + "\t\t-pageid postの場合必須" + "\r\n"
                + "\t\t-f 出力先のファイルパスを指定します" + "\r\n"
                , Backno),
            new WpapiAction("/pageupdate", "固定ページの投稿" + "\r\n"
                + "\t\t-pageid 必須" + "\r\n"
                + "\t\t-f 記事の内容が記載されたファイルパスを指定します" + "\r\n"
                , PageUpdate),
        };
        #endregion

        #region ヘルプ
        /// <summary>
        /// ヘルプ
        /// </summary>
        /// <param name="action">アクション名</param>
        public static void Help(string action)
        {
            try
            {
                Console.WriteLine("使用方法：");
                Console.WriteLine("\ttwapi /actioncommand [-options]");

                Console.WriteLine("");
                Console.WriteLine("actioncommand :");

                foreach (var tmp in WpapiActions.Actions)
                {
                    Console.WriteLine($"\t{tmp.ActionName}\t...{tmp.Help}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.Error(e.Message);
                throw;
            }
        }
        #endregion

        #region キーの登録処理
        /// <summary>
        /// キーの登録処理
        /// </summary>
        /// <param name="action">アクション名</param>
        public static void Regist(string action)
        {
            try
            {
                XMLUtil.Seialize<WpapiKeys>(ConfigManager.KeysFile, WordpressAPI.WpKeys);
                Console.WriteLine("各種キーを保存しました");
                Console.WriteLine("==>" + ConfigManager.KeysFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region ページの更新処理
        /// <summary>
        /// ページの更新処理
        /// </summary>
        /// <param name="action">アクション名</param>
        public static void PageUpdate(string action)
        {
            try
            {
                int page_id = -1;
                // ファイル名が指定されていればそこに出力
                if (!string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.PageId))
                {
                    page_id = int.TryParse(WpapiArgs.CommandOptions.PageId, out page_id) ? page_id : -1;
                }
                else
                {
                    Console.WriteLine("page_idは必須です。");
                    return;
                }

                string content = string.Empty;
                if (!string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.FileName)
                    && File.Exists(WpapiArgs.CommandOptions.FileName))
                {
                    using (FileStream fs = new FileStream(WpapiArgs.CommandOptions.FileName,
                           FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (TextReader sr = new StreamReader(fs))
                        {
                            content = sr.ReadToEnd();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("有効なファイルパスが指定されていません。");
                    Console.WriteLine("ファイルパスは必須です。");
                    Console.WriteLine(WpapiArgs.CommandOptions.FileName);
                    return;
                }

                // ページの投稿
                PostData(page_id, content).Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region バックナンバー作成処理
        /// <summary>
        /// バックナンバー作成処理
        /// </summary>
        /// <param name="action">アクション名</param>
        public static void Backno(string action)
        {
            try
            {
                switch (WpapiArgs.CommandOptions.Type)
                {
                    case "output":
                        {
                            // バックナンバー出力処理
                            BacknoOutput();
                            break;
                        }
                    case "post":
                        {
                            // 固定記事の投稿
                            BacknoPost();
                            break;
                        }
                }
            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region バックナンバーの投稿処理
        /// <summary>
        /// バックナンバーの投稿処理
        /// </summary>
        private static void BacknoPost()
        {
            GetAllPost().Wait();

            string html, html2 = string.Empty;

            bool showid = !string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.ShowId);

            // ソート順を日付で指定の場合
            if (!string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.Sort)
                && WpapiArgs.CommandOptions.Sort.Equals("date"))
            {
                // 日付順でソートしてHTMLを作成
                html = GetHtmlDate(showid);
            }
            else
            {
                // タイトル順でソートしてHTMLを作成
                html = GetHtmlTitle(showid);
            }

            // ファイル名が指定されていればそこに出力
            if (!string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.PageId))
            {
                int page_id = int.TryParse(WpapiArgs.CommandOptions.PageId, out page_id) ? page_id : -1;

                // ページの投稿
                PostData(page_id, html).Wait();
            }
        }
        #endregion

        #region バックナンバーの出力処理
        /// <summary>
        /// バックナンバーの出力処理
        /// </summary>
        private static void BacknoOutput()
        {
            GetAllPost().Wait();

            string html, html2 = string.Empty;

            bool showid = !string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.ShowId);

            // ソート順を日付で指定の場合
            if (!string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.Sort)
                && WpapiArgs.CommandOptions.Sort.Equals("date"))
            {
                // 日付順でソートしてHTMLを作成
                html = GetHtmlDate(showid);
            }
            else
            {
                // タイトル順でソートしてHTMLを作成
                html = GetHtmlTitle(showid);
            }

            // コンソール表示
            Console.WriteLine(html);

            // ファイル名が指定されていればそこに出力
            if (!string.IsNullOrWhiteSpace(WpapiArgs.CommandOptions.FileName))
            {
                File.WriteAllText(WpapiArgs.CommandOptions.FileName, html);
            }
        }
        #endregion

        static List<Post> _Posts;
        static List<Page> _Pages;

        #region 全記事の取得
        /// <summary>
        /// 全記事の取得
        /// </summary>
        /// <returns>Task</returns>
        private static async Task GetAllPost()
        {
            try
            {
                // 記事一覧の取得
                var post = await WordpressAPI.WpClient.Posts.GetAll();
                _Posts = new List<Post>(post);

                // 固定ページ一覧の取得
                var page = await WordpressAPI.WpClient.Pages.GetAll();
                _Pages = new List<Page>(page);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
        }
        #endregion

        #region バックナンバーをタイトル順でHTML出力する
        /// <summary>
        /// バックナンバーをタイトル順でHTML出力する
        /// </summary>
        /// <returns>html</returns>
        private static string GetHtmlTitle(bool show_id)
        {
            StringBuilder text = new StringBuilder();

            text.AppendLine($"更新日：{DateTime.Now.ToString("yyyy年MM月dd日")}");

            text.AppendLine("");

            text.AppendLine("## 投稿一覧");

            _Posts = (from x in _Posts
                      orderby x.Title.Rendered
                      select x).ToList();

            foreach (var post in _Posts)
            {
                text.AppendLine($"[{post.Title.Rendered.Replace("&#8211;", "-")}]({post.Link})" + (show_id ? $" 記事id={post.Id}" : string.Empty));
            }

            text.AppendLine("");
            text.AppendLine("## 固定ページ一覧");

            _Pages = (from x in _Pages
                      orderby x.Title.Rendered
                      select x).ToList();

            foreach (var page in _Pages)
            {
                text.AppendLine($"[{page.Title.Rendered.Replace("&#8211;", "-")}]({page.Link})" + (show_id ? $" 記事id={page.Id}" : string.Empty));
            }

            //convert Mark down to html and set to mdContents
            Markdig.MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().UsePipeTables().Build();

            return Markdown.ToHtml(text.ToString(), markdownPipeline);
        }
        #endregion

        #region バックナンバーを日付順にHTMLで出力する
        /// <summary>
        /// バックナンバーを日付順にHTMLで出力する
        /// </summary>
        /// <returns>HTML</returns>
        private static string GetHtmlDate(bool show_id)
        {
            StringBuilder text = new StringBuilder();

            text.AppendLine($"更新日：{DateTime.Now.ToString("yyyy年MM月dd日")}");

            text.AppendLine("");

            text.AppendLine("## 投稿一覧");

            _Posts = (from x in _Posts
                      orderby x.Date
                      select x).ToList();

            DateTime tmp_date = DateTime.MinValue;

            foreach (var post in _Posts)
            {
                // 年 or 月が異なる場合
                if (!post.Date.Year.Equals(tmp_date.Year) || !post.Date.Month.Equals(tmp_date.Month))
                {
                    tmp_date = post.Date;
                    text.AppendLine($"### {tmp_date.ToString("yyyy年MM月")}");
                }

                text.AppendLine($"[{post.Title.Rendered.Replace("&#8211;", "-")}]({post.Link})" + (show_id ? $" 記事id={post.Id}" : string.Empty));
            }

            text.AppendLine("");
            text.AppendLine("## 固定ページ一覧");

            _Pages = (from x in _Pages
                      orderby x.Date
                      select x).ToList();

            tmp_date = DateTime.MinValue;

            foreach (var page in _Pages)
            {
                // 年 or 月が異なる場合
                if (!page.Date.Year.Equals(tmp_date.Year) || !page.Date.Month.Equals(tmp_date.Month))
                {
                    tmp_date = page.Date;
                    text.AppendLine($"### {tmp_date.ToString("yyyy年MM月")}");
                }

                text.AppendLine($"[{page.Title.Rendered.Replace("&#8211;", "-")}]({page.Link})" + (show_id ? $" 記事id={page.Id}" : string.Empty));
            }

            //convert Mark down to html and set to mdContents
            Markdig.MarkdownPipeline markdownPipeline = new MarkdownPipelineBuilder().UsePipeTables().Build();

            return Markdown.ToHtml(text.ToString(), markdownPipeline);
        }
        #endregion

        #region 固定ページへの投稿処理
        /// <summary>
        /// 固定ページへの投稿処理
        /// </summary>
        /// <param name="page_id">ページID</param>
        /// <param name="contents">投稿記事内容</param>
        /// <returns>タスク</returns>
        private static async Task PostData(int page_id, string contents)
        {
            var pagetmp = await WordpressAPI.WpClient.Pages.GetByID(page_id);
            pagetmp.Date = DateTime.Now;
            pagetmp.Content = new Content(contents);

            if (await WordpressAPI.WpClient.IsValidJWToken())
            {
                await WordpressAPI.WpClient.Pages.Update(pagetmp);
            }
        }
        #endregion

    }
}
