using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Domain.Models
{
    public class UserSettings
    {
        public int Id { get; set; }

        #region InternalEnums
        public enum GeneratingColors
        {
            Random,
            ColorShadesHarmony,
            WithColorPicker
        }

        public enum Theme
        {
            White,
            Black
        }
        #endregion

        #region AppSettings
        public GeneratingColors GeneratingColorsWay { get; set; }
        public Theme AppTheme { get; set; }
        public bool IsSynchronizationOn { get; set; }
        #endregion

        #region TaskSettings
        public bool AutoMoveTasksThatExpired { get; set; }
        #endregion

        public UserSettings()
        {
            DefaultSettings();
        }

        private void DefaultSettings()
        {
            GeneratingColorsWay = GeneratingColors.Random;
            AppTheme = Theme.White;
            IsSynchronizationOn = true;
            AutoMoveTasksThatExpired = false;
        }

        public void Reset()
        {
            DefaultSettings();
        }
    }
}
