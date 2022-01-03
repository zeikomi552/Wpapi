using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpapi.Models.Wordpress
{
    public class WpapiCommandOptions : WpapiBase
    {
        /// <summary>
        /// WordPressのユーザーアカウント
        /// </summary>
        [EndpointParam("-u")]
        public string UserAccount { get; set; }

        /// <summary>
        /// WordPressのパスワード
        /// </summary>
        [EndpointParam("-p")]
        public string Password { get; set; }

        /// <summary>
        /// WordPressを使用しているURL 例：https://www.premium-tsubu-hero.net/
        /// </summary>
        [EndpointParam("-url")]
        public string Url { get; set; }

        /// <summary>
        /// キーファイルパス
        /// </summary>
        [EndpointParam("-keysfile")]
        public string KeysFile { get; set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        [EndpointParam("-f")]
        public string FileName { get; set; }

        /// <summary>
        /// タイプ
        /// </summary>
        [EndpointParam("-type")]
        public string Type { get; set; }

        /// <summary>
        /// ページID
        /// </summary>
        [EndpointParam("-pageid")]
        public string PageId { get; set; }

        /// <summary>
        /// ソート順
        /// </summary>
        [EndpointParam("-sort")]
        public string Sort { get; set; }

        /// <summary>
        /// 記事Idの表示
        /// </summary>
        [EndpointParam("-showid")]
        public string ShowId { get; set; }
    }
}
