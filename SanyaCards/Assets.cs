using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SanyaCards
{
    internal class Assets
    {
        private static readonly AssetBundle Bundle = Jotunn.Utils.AssetUtils.LoadAssetBundleFromResources("sanyacards", typeof(SanyaCards).Assembly);

        public static AudioClip eatingSandwich = Bundle.LoadAsset<AudioClip>("NomNomNom.mp3");
    }
}
