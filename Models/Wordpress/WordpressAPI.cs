using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;

namespace Wpapi.Models.Wordpress
{
    public class WordpressAPI : WpapiBase
    {
        public static WordPressClient WpClient = null;

        /// <summary>
        /// APIキー
        /// </summary>
        public static WpapiKeys WpKeys { get; set; } = new WpapiKeys();

        #region 接続用クライアントの作成
        /// <summary>
        /// 接続用クライアントの作成
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="username">ワードプレスのユーザー名</param>
        /// <param name="password">ワードプレスのパスワード</param>
        /// <returns>Task</returns>
        public static async Task CreateClient(string url, string username, string password)
        {
            if (WpClient == null)
            {
                // JWT authentication
                WpClient = new WordPressClient($"{url}/wp-json/");
                WpClient.AuthMethod = AuthMethod.JWT;
                await WpClient.RequestJWToken(username, password);
            }
        }
        #endregion
    }
}
