﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpapi.Models;
using Wpapi.Models.Wordpress;

namespace Wpapi
{
    public class WpapiCommand : WpapiBase
    {
        #region Action
        /// <summary>
        /// Action
        /// </summary>
        static string _Action = string.Empty;
        #endregion

        #region SQLiteのファイルパス
        /// <summary>
        /// SQLiteのファイルパス
        /// </summary>
        public static string SQLitePath { get; set; }
        #endregion

        #region コマンドライン引数
        /// <summary>
        /// コマンドライン引数
        /// </summary>
        public static WpapiArgs WpapiArgs { get; set; }
        #endregion

        #region コマンドの実行
        /// <summary>
        /// コマンドの実行
        /// </summary>
        /// <param name="args">パラメータ</param>
        public static void ExecuteCommand(string[] args)
        {
            try
            {
                // 引数のセット
                WpapiArgs.SetCommand(args);

                string action = string.Empty;

                if (WpapiArgs.Args.ContainsKey("action"))
                {
                    action = WpapiArgs.Args["action"];
                }
                else
                {
                    Console.WriteLine("不正なコマンドです。\r\n");
                    action = "/?";
                }

                // アクションの実行
                foreach (var tmp in WpapiActions.Actions)
                {
                    if (action.Equals(tmp.ActionName))
                    {
                        tmp.Action(tmp.ActionName);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                Console.WriteLine(e.Message);
                throw;
            }
        }
        #endregion
    }
}
