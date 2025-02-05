using Newtonsoft.Json;
using System.IO;
using static Program;

namespace TextRPG
{
    internal class Player
    {
        public int Level { get; private set; }
        public int Exp { get; private set; }
        public string Name { get; }
        public JobType JobType { get; }
        public int Attack { get; private set; }
        public int AdditionalAttack { get; private set; }
        public int Defence { get; private set; }
        public int AdditionalDefence { get; private set; }
        public int Health { get; set; }
        public int Gold { get; set; }

        public int[,] ItemSaveInfo = new int [7, 2]; 

        public List<Item> ItemList = new();
        public Item Armor { get; private set; }
        public Item Weapon { get; private set; }
        
        public Player(string n, JobType jT)
        {
            this.Level = 1;
            this.Name = n;
            this.JobType = jT;
            this.Gold = 3000;
            this.AdditionalAttack = 0;
            this.AdditionalDefence = 0;
            switch (jT)
            {
                case JobType.전사:
                    this.Attack = 10;
                    this.Defence = 5;
                    this.Health = 100;
                    break;
                case JobType.도적:
                    this.Attack = 20;
                    this.Defence = 3;
                    this.Health = 50;
                    break;
                default: break;
            }
        }

        public bool SetItemList(ItemName iN,int OneOrNegOne)//OneOrNegOne 사는경우 1 : 파는경우 -1
        {
            var item = ItemList.Find(g => g.name == iN);
            if (OneOrNegOne == 1)
            {
              if(item == null)//사는데 아이템이 null일 경우
                {
                    ItemList.Add(new Item(iN, 1));
                    ItemList.Sort((x, y) => x.name.CompareTo(y.name));
                    var temp = ItemList.Find(g => g.name == iN);
                    if (temp.value <= Gold) { Gold -= temp.value; return true; }
                    else 
                    { 
                        ItemList.Remove(temp);
                        ItemList.Sort((x, y) => x.name.CompareTo(y.name));
                        return false;
                    }
                }
                else//사는데 아이템이 있는경우
                {
                    if (item.value <= Gold) 
                    { 
                        Gold -= item.value;
                        item.count++;
                        return true; 
                    }
                    else return false;
                }
            }
            else 
            {
                float sellRatio = 0.85f;
                if(item.count == 1)//파는데 마지막 아이템인 경우
                {
                    Gold += (int)(item.value * sellRatio);
                    if (item.isEquip == true) SetEquipment(item);//장착된 장비라면 해제
                    ItemList.Remove(item);
                    ItemList.Sort((x, y) => x.name.CompareTo(y.name));
                    return true;
                }
                else //여러개 보유중 파는경우
                {
                    Gold += (int)(item.value * sellRatio);
                    item.count--;
                    return true;
                }
            } 
        }
        public void SetEquipment(Item i)
        {
            if (i.type == ItemType.방어구)
            {
                if (i== Armor) //같은 방어구 장착시도 시 해제
                {
                    Armor.isEquip = false;
                    Armor = null;
                    AdditionalDefence = 0;
                }
                else //다른 방어구 장착
                {
                    if (Armor != null)
                    {
                        AdditionalDefence -= Armor.option;
                        Armor.isEquip = false;
                    }
                    Armor = i;
                    AdditionalDefence += Armor.option;
                    Armor.isEquip = true;
                }
                    
            }
            else
            {
                if (i == Weapon) //같은 무기 장착시도 시 해제
                {
                    Weapon.isEquip = false;
                    Weapon = null;
                    AdditionalAttack = 0;
                }
                else //다른 무기 장착
                {
                    if (Weapon != null)
                    {
                        AdditionalAttack -= Weapon.option;
                        Weapon.isEquip = false;
                    }
                    Weapon = i;
                    AdditionalAttack += Weapon.option;
                    Weapon.isEquip = true;
                }
            }
        }

        public void AddExp()
        {
            if (Exp < Level) Exp++;//1레밸 경험치1, 2레밸 경험치2, ...
            else
            {
                if (Level % 2 == 0) Attack++; //2레벨마다 1증가
                Defence++;
                Level++;
            }
        }

        public void ReadyToSave()//세이브하기 위해 ItemSaveInfo 기록
        {
            int i = 0;
            foreach (ItemName e in Enum.GetValues(typeof(ItemName)))
            {
                ItemSaveInfo[i, 0] = (int)e;
                Item item = ItemList.Find(g => g.name == e);
                ItemSaveInfo[i, 1] = (item != null) ? item.count : 0;
                i++;
            }
            // 장착 아이템 저장 (없으면 -1)
            ItemSaveInfo[6, 0] = (Armor != null) ? (int)Armor.name : -1;
            ItemSaveInfo[6, 1] = (Weapon != null) ? (int)Weapon.name : -1;
        }
        public void LoadItem()//ItemSaveInfo로 아이템 로드
        {
            ItemList.Clear();
            for (int i = 0; i < Enum.GetValues(typeof(ItemName)).Length; i++)
            {
                int itemId = ItemSaveInfo[i, 0];
                int itemCount = ItemSaveInfo[i, 1];
                if (itemCount > 0)ItemList.Add(new Item((ItemName)itemId, itemCount));
            }
            // 장착 아이템 로드
            Armor = (ItemSaveInfo[6, 0]!= -1) ? ItemList.Find(g => (int)g.name == ItemSaveInfo[6, 0]) : null;
            Weapon = (ItemSaveInfo[6, 1] != -1) ? ItemList.Find(g => (int)g.name == ItemSaveInfo[6, 1]) : null;
        }
        public void SaveToJson(string filePath) //PlayerSaveData 생성 활용하여 filePath에 저장
        {
            ReadyToSave();
            PlayerSaveData saveData = new PlayerSaveData 
            {
                Level = this.Level,
                Exp = this.Exp,
                Name = this.Name,
                JobType = this.JobType,
                Attack = this.Attack,
                AdditionalAttack = this.AdditionalAttack,
                Defence = this.Defence,
                AdditionalDefence = this.AdditionalDefence,
                Health = this.Health,
                Gold = this.Gold,
                ItemSaveInfo = this.ItemSaveInfo
            };
            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented); //시리얼라이즈
            File.WriteAllText(filePath, json); //텍스트 기입
        }
        public static Player LoadFromJson(string filePath) 
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("파일이 존재하지 않습니다.");
                return null;
            }

            string json = File.ReadAllText(filePath); //filePath에서 json으로 Read

            PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json); //디시리얼라이즈

            Player loadedPlayer = new Player(saveData.Name, saveData.JobType)
            {
                Level = saveData.Level,
                Exp = saveData.Exp,
                Attack = saveData.Attack,
                AdditionalAttack = saveData.AdditionalAttack,
                Defence = saveData.Defence,
                AdditionalDefence = saveData.AdditionalDefence,
                Health = saveData.Health,
                Gold = saveData.Gold,
                ItemSaveInfo = saveData.ItemSaveInfo
            };

            loadedPlayer.LoadItem();//saveInfo -> ItemList & Equipment
             
            return loadedPlayer;
        }
    }  
}
