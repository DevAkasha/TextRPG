

namespace TextRPG
{
    internal class Item
    {
        public ItemName name;
        public ItemType type;
        public int option;
        public string explain;
        public int value;
        public bool isEquip = false;
        public int count = 1;

        public Item(ItemName n, int c)
        {
            name = n;
            type = ((int)n >= 100) ? ItemType.무기 : ItemType.방어구;
            switch (n)
            {
                case ItemName.수련자_갑옷: option = 5; value = 2000; explain = "수련에 도움을 주는 갑옷입니다."; count = c; break;
                case ItemName.무쇠갑옷: option = 9; value = 4000; explain = "무쇠로 만들어져 튼튼한 갑옷입니다."; count = c; break;
                case ItemName.스파르타의_갑옷: option = 15; value = 7000; explain = "스파르타의 전사들이 사용했다는 전설의 갑옷"; count = c; break;
                case ItemName.낡은_검: option = 2; value = 1200; explain = "쉽게 볼 수 있는 낡은 검입니다."; count = c; break;
                case ItemName.청동_도끼: option = 5; value = 2000; explain = "어디선가 사용됐던거 같은 도끼입니다."; count = c; break;
                case ItemName.스파르타의_창: option = 7; value = 3000; explain = "스파르타의 전사들이 사용했다던 전설의 창입니다."; count = c; break;
            }
        }
    }
    enum ItemName {수련자_갑옷,무쇠갑옷,스파르타의_갑옷,낡은_검=100,청동_도끼,스파르타의_창 }
    enum ItemType { 무기, 방어구 }
}
