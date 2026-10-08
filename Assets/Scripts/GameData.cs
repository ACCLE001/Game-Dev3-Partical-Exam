using UnityEngine;

namespace TanesHemburud
{
    public static class GameData
    {
        public const string StudentCode = "672110095";
        public const string StudentName = "ธเณศร์ เห็มบุรุษ";
        public const string StudentNameEnglish = "Tanes Hemburud";

        public static int CurrentScore = 0;
        public static string LastPlayedStage = "Stage1";

        public static float BGMVolume = 1.0f;
        public static float SFXVolume = 1.0f;

        public static void ResetGameData()
        {
            CurrentScore = 0;
        }
    }
}
