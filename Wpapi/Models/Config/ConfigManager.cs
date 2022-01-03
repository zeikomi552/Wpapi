﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpapi.Models.Utilities;

namespace Wpapi.Models.Config
{
    class ConfigManager
    {
        #region Configファイル用フォルダパス
        /// <summary>
        /// Configファイル用フォルダパス
        /// </summary>
        public static string ConfigDir
        {
            get
            {
                string path = PathManager.GetApplicationFolder();
                string config_dir = Path.Combine(path, "Config");
                PathManager.CreateDirectory(config_dir);
                return config_dir;
            }
        }
        #endregion

        #region キーファイルパス
        /// <summary>
        /// キーファイルパス
        /// </summary>
        public static string KeysFile { get; set; } = Path.Combine(ConfigManager.ConfigDir, "wpapi.keys");
        #endregion

    }
}
