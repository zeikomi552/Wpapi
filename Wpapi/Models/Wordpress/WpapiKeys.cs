using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpapi.Models.Wordpress
{
    public class WpapiKeys : WpapiBase
    {
        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WpapiKeys()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="user_id">ワードプレスのユーザーID</param>
        /// <param name="password">ワードプレスのパスワード</param>
        public WpapiKeys(string url, string user_id, string password)
        {
            this.Url = url;
            this.UserAccount = user_id;
            this.Password = password;
        }
        #endregion

        #region URL
        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }
        #endregion

        #region ユーザーアカウント
        /// <summary>
        /// ユーザーアカウント
        /// </summary>
        public string UserAccount { get; set; }
        #endregion

        #region パスワード
        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; set; }
        #endregion
    }
}
