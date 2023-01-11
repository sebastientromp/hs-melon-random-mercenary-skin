using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomMercenarySkin
{
    public class Utils
    {
        public struct MercenarySkin
        {
            public string Name;
            public List<MercenarySkinVariation> Variations;
            public bool hasDiamond;
            public int Diamond;
            public int Default;
        }

        public struct MercenarySkinVariation
        {
            public int ID;
            public TAG_PREMIUM Premium;

            public override string ToString()
            {
                return $"dbfId={ID}, premium={Premium}";
            }
        }

        public static List<MercenarySkin> CacheMercenarySkin = new List<MercenarySkin>();

        public static class CacheInfo
        {
            public static void UpdateMercenarySkin()
            {
                CacheMercenarySkin.Clear();
                var allMercenaries = CollectionManager.Get().FindMercenaries().m_mercenaries;
                foreach (var merc in allMercenaries)
                {
                    if (merc != null && merc.m_artVariations.Count > 0)
                    {
                        MercenarySkin mercSkin = new MercenarySkin
                        {
                            Name = merc.m_artVariations.First().m_record.CardRecord.Name.GetString(),
                            Variations = new List<MercenarySkinVariation>(),
                        };
                        foreach (var art in merc.m_artVariations.OrderBy(x => x.m_record.ID).ToList())
                        {
                            if (art != null)
                            {
                                var variation = new MercenarySkinVariation {
                                    ID = art.m_record.CardId,
                                    Premium = art.m_premium,
                                };
                                mercSkin.Variations.Add(variation);
                            }
                        }
                        CacheMercenarySkin.Add(mercSkin);
                    }
                }
            }
        }

        public static bool IsMercenarySkin(string cardID, out MercenarySkin skin)
        {
            if (CacheMercenarySkin.Count == 0)
            {
                CacheInfo.UpdateMercenarySkin();
            }
            int dbid = GameUtils.TranslateCardIdToDbId(cardID);

            foreach (var mercSkin in CacheMercenarySkin)
            {
                if (mercSkin.Variations.Select(v => v.ID).Contains(dbid))
                {
                    skin = mercSkin;
                    return true;
                }
            }
            skin = new MercenarySkin();
            return false;
        }
    }
}
