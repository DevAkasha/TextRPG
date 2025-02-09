﻿using System.IO;
using TextRPG;
internal class Program
{
    static class ViewUtil//View에서 필요한 메소드 도구 모음클래스
    {
        public static void PrintTitle(ViewTitle title)
        {
            Console.Clear();
            Console.WriteLine(title.ToString());
        }
        public static void PrintNotice(string[] notice)
        {
            foreach (string s in notice) { Console.WriteLine(s); }
            Console.WriteLine("");
        }
        public static void PrintSelection<T>(T[] selection)
        {
            for (int i = 1; i <= selection.Length; i++)
            {
                Console.WriteLine($"{i}. {selection[i - 1]}");
            }
            Console.WriteLine("");
        }
        public static void PrintPlayerView(Player player)
        { 
            Console.WriteLine($"Lv. {player.Level}");
            Console.WriteLine($"{player.Name}( {player.JobType} )");
            Console.WriteLine($"공격력 : {player.Attack}+({player.AdditionalAttack})");
            Console.WriteLine($"방어력 : {player.Defence}+({player.AdditionalDefence})");
            if(player.Health<0) Console.WriteLine($"체력 : {player.Health}(언데드)");//언데드 상태에 던전입장불가
            Console.WriteLine($"체력 : {player.Health}");
            Console.WriteLine($"Gold : {player.Gold}");
            Console.WriteLine("");
        }
        public static int GetUserInput(int minCount, int selectionCount)
        {
            if (minCount == 0) Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.");
            Console.Write(">>");
            //커서복귀를 위한 커서위치 저장
            int x = Console.CursorLeft;
            int y = Console.CursorTop;

            int result;
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out result))
                {
                    if (result <= selectionCount && result >= minCount)//유효한 입력
                    {
                        if(result == 0) CallView(ViewTitle.마을);
                        else return result;
                    } 
                    else Console.WriteLine($"{minCount}에서{selectionCount}사이의 숫자를 입력해주세요");
                }
                Console.SetCursorPosition(x,y);
            }
        }
        public static void PrintItemListView(List<Item> itemList,ViewTitle title)//다이나믹용 호출 뷰타이틀 매개변수
        {
            Console.WriteLine("[아이템 목록]");
            if (itemList.Count != 0)
            {
                int[] x = {4,23,35,92,102}; //요소들 배치를 위한 x좌표들
                int y = Console.CursorTop;  //현재 y좌표

                int i = 0;
                foreach (Item item in itemList) 
                {
                    //뷰 다이나믹용 조건문
                    if (title==ViewTitle.인벤토리|| title == ViewTitle.상점) Console.Write("- "); 
                    else if(title == ViewTitle.장착관리|| title == ViewTitle.상점구매|| title == ViewTitle.상점판매) Console.Write($"- {i+1}.");
                    Console.SetCursorPosition(x[0], y+i);
                    if (title == ViewTitle.상점구매)  Console.Write($"{item.name}");
                    else
                    {
                        if (item.isEquip) Console.Write($"[E]{item.name}");
                        else Console.Write($"{item.name}");
                    }             
                    Console.SetCursorPosition(x[1], y + i);
                    if (item.type==ItemType.방어구) Console.Write($"I방어력+{item.option}"); 
                    else Console.Write($"|공격력+{item.option}");
                    Console.SetCursorPosition(x[2], y + i);
                    Console.Write($"|{item.explain}");
                    Console.SetCursorPosition(x[3], y + i);
                    Console.Write($"|{item.value}G");
                    Console.SetCursorPosition(x[4], y + i);
                    Console.WriteLine($"|{item.count}");
                    ++i;
                } 
            }
            Console.WriteLine("");
        }
    }

    enum ViewTitle { 마을, 상태보기, 인벤토리, 장착관리, 상점, 상점구매, 상점판매, 던전입장, 던전결과,휴식하기  }
    // ViewTitle : View마다 한개씩 구현, View간 이동에도 사용된다.
    static void CallView(ViewTitle viewTitle)//View이동 메소드
    {
        switch (viewTitle)
        {
            case ViewTitle.마을: HomeView(); break;
            case ViewTitle.상태보기: StateView(); break;
            case ViewTitle.인벤토리: InventoryView(); break;
            case ViewTitle.장착관리: EquipmentView(); break;
            case ViewTitle.상점: StoreView(); break;
            case ViewTitle.상점구매: StoreBuyView(); break;
            case ViewTitle.상점판매: StoreSellView(); break;
            case ViewTitle.던전입장: DungeonView(); break;
          //case ViewTitle.던전결과: 던전결과뷰는 던전에서만 호출된다.
            case ViewTitle.휴식하기: RestView(); break; 
            default: HomeView(); break;
        }
    }

    public static string ReceiveNameView()
    {
        string name;
        string[] notice = ["스파르타 던전에 오신 여러분 환영합니다.",
                    "원하시는 이름을 설정해 주세요."];
        string[] selection = ["저장", "취소"];
        
        Console.Clear();
        ViewUtil.PrintNotice(notice);
        name = Console.ReadLine();
        if(string.IsNullOrWhiteSpace(name)) return ReceiveNameView();
        Console.WriteLine($"입력하신 이름은 {name}입니다.");
        ViewUtil.PrintSelection(selection);
        switch (ViewUtil.GetUserInput(1, selection.Length))
        {
            case 1: return name;
            default: return ReceiveNameView();
        }
    }
    public static JobType SelectJobView()
    {
        string[] notice = ["스파르타 던전에 오신 여러분 환영합니다.",
                    "원하시는 직업을 설정해 주세요."];
        JobType[] selection = [JobType.전사, JobType.도적];

        Console.Clear();
        ViewUtil.PrintNotice(notice);
        for (int i = 1; i <= selection.Length; i++)
        {
            Console.WriteLine($"{i}. " + selection[i - 1]);
        }
        Console.WriteLine("");

        switch (ViewUtil.GetUserInput(1, selection.Length))
        {
            case 1: return JobType.전사;
            case 2: return JobType.도적;
            default: return JobType.전사;
        }
    }
    public static void SelectLoadView()
    {
        string[] notice = ["스파르타 던전에 또 오셨군요?",
                    "이어하시겠습니까?"];
        string[] selection = ["이어하기", "새로하기"];

        Console.Clear();
        ViewUtil.PrintNotice(notice);
        ViewUtil.PrintSelection(selection);
        switch(ViewUtil.GetUserInput(1, selection.Length))
        {
            case 1: GameManager.I().isLoadGame = true; break;
            default: File.Delete(filePath); break;//이 뷰는 시작때만 오기때문에 isLoadGame=false 생략
        }
    }

    static void HomeView()
    {
        ViewTitle title = ViewTitle.마을;
        string[] notice = ["스파르타 마을에 오신 여러분 환영합니다.",
                    "이곳에서 던전으로 들어가기전 활동을 할 수 있습니다."];
        ViewTitle[] selection = [ViewTitle.상태보기, ViewTitle.인벤토리, ViewTitle.상점, ViewTitle.던전입장,ViewTitle.휴식하기];

        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        ViewUtil.PrintSelection(selection);
        Console.WriteLine("6. 저장하기\n"); 
        int userInput = ViewUtil.GetUserInput(1, 6);
        //6선택시 저장하고 갱신
        if (userInput == 6) { GameManager.I().GetPlayer().SaveToJson(filePath); CallView(title); }
        CallView(selection[userInput - 1]);//셀렉션 순서에 맞는 뷰 호출
    }

    static void StateView()
    {
        ViewTitle title = ViewTitle.상태보기;
        string[] notice = ["캐릭터의 정보가 표시됩니다."];
        
        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        ViewUtil.PrintPlayerView(GameManager.I().GetPlayer());
        ViewUtil.GetUserInput(0, 0);//마을가기 입력받기
    }

    static void InventoryView()
    {
        ViewTitle title = ViewTitle.인벤토리;
        string[] notice = ["보유 중인 아이템을 관리할 수 있습니다."];
        ViewTitle[] selection = [ViewTitle.장착관리];

        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        ViewUtil.PrintItemListView(GameManager.I().GetPlayer().ItemList,title);
        ViewUtil.PrintSelection(selection);
        int userInput = ViewUtil.GetUserInput(0, 1);
        CallView(selection[userInput - 1]);
    }
    static void EquipmentView() 
    {
        ViewTitle title = ViewTitle.장착관리;
        string[] notice = ["보유 중인 아이템을 관리할 수 있습니다."];
        List<Item> equipList = GameManager.I().GetPlayer().ItemList;

        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        ViewUtil.PrintItemListView(equipList, title);
        int userInput = ViewUtil.GetUserInput(0, equipList.Count);
        GameManager.I().GetPlayer().SetEquipment(equipList[userInput-1]);
        CallView(title);
    }
    static void StoreView()
    {
        ViewTitle title = ViewTitle.상점;
        string[] notice = ["아이템을 사고 팔 수 있는 상점입니다."];
        ViewTitle[] selection = [ViewTitle.상점구매, ViewTitle.상점판매];
        List<Item> ShowCase = new List<Item>();//쇼케이스, 카운트 0이어도 보여주기 위한 리스트
        List<Item> PlayerItemList = GameManager.I().GetPlayer().ItemList;
        //쇼케이스에 각 아이템 하나씩 진열
        foreach (ItemName e in Enum.GetValues(typeof(ItemName))) ShowCase.Add(new Item(e,1));
        //쇼케이스에 보유count 대입
        foreach (Item i in ShowCase)
        {
            var foundItem = PlayerItemList.Find(g => g.name == i.name);
            i.count = foundItem == null ? 0 : foundItem.count;
        }
        
        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        Console.WriteLine($"[보유 골드]\n{GameManager.I().GetPlayer().Gold} G");
        ViewUtil.PrintItemListView(ShowCase, title);
        ViewUtil.PrintSelection(selection);
        int userInput = ViewUtil.GetUserInput(0, selection.Length);
        CallView(selection[userInput - 1]);
    }
    static void StoreBuyView() 
    {
        ViewTitle title = ViewTitle.상점구매;
        string[] notice = ["필요한 아이템을 구입할 수 있습니다."];

        List<Item> ShowCase = new List<Item>();
        List<Item> PlayerItemList = GameManager.I().GetPlayer().ItemList;
        //쇼케이스에 각 아이템 하나씩 진열
        foreach (ItemName e in Enum.GetValues(typeof(ItemName))) ShowCase.Add(new Item(e, 1));
        //쇼케이스에 보유count 대입
        foreach (Item i in ShowCase) i.count = PlayerItemList.Find(g => g.name == i.name) == null ? 0 : PlayerItemList.Find(g => g.name == i.name).count;

        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        Console.WriteLine($"[보유 골드]\n{GameManager.I().GetPlayer().Gold} G");
        ViewUtil.PrintItemListView(ShowCase, title);     
        int userInput = ViewUtil.GetUserInput(0, ShowCase.Count);
        //SetItemList호출 : 거래 성공bool값 반환, 반환전 거래 로직 동작
        if (!GameManager.I().GetPlayer().SetItemList(ShowCase[userInput-1].name, +1))
        {
            Console.WriteLine("\n소지금액이 부족합니다.");
            Thread.Sleep(800);
        }
        CallView(title);
    }
    static void StoreSellView()
    {
        ViewTitle title = ViewTitle.상점판매;
        string[] notice = ["필요없는 아이템을 판매할 수 있습니다."];
        List<Item> PlayerItemList = GameManager.I().GetPlayer().ItemList;

        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        Console.WriteLine($"[보유 골드]\n{GameManager.I().GetPlayer().Gold} G");
        ViewUtil.PrintItemListView(PlayerItemList, title);
        int userInput = ViewUtil.GetUserInput(0, PlayerItemList.Count);
        //SetItemList호출 : 거래 성공bool값 반환, 반환전 거래 로직 동작
        if (!GameManager.I().GetPlayer().SetItemList(PlayerItemList[userInput - 1].name, -1))
        {
            Console.WriteLine("\n판매에 실패했습니다.");
            Thread.Sleep(800);
        }
        CallView(title);
    }
    static void DungeonView()
    {
        if (GameManager.I().GetPlayer().Health < 0)
        {
            Console.WriteLine("언데드는 던전에 입장할 수 없습니다. \n회복하고 오세요");
            Thread.Sleep(1200);
            CallView(ViewTitle.마을);
        }
        ViewTitle title = ViewTitle.던전입장;
        string[] notice = ["들어갈 던전을 선택해 주세요"];
        string[] selection = ["쉬운 던전 \t| 방어력 5이상 권장",
                               "일반 던전 \t| 방어력 11이상 권장",
                                "어려운 던전 \t| 방어력 17이상 권장"];
        ViewUtil.PrintTitle(title);
        ViewUtil.PrintNotice(notice);
        ViewUtil.PrintSelection(selection);
        int userInput = ViewUtil.GetUserInput(0, selection.Length);
        DungeonResultView(userInput);
    }
    static void DungeonResultView(int dungeonLv)
    {
        string[] doungeonName = ["쉬운 던전", "일반 던전", "어려운 던전"];
        int[,] dungeonBalance = new int[3,2] { { 5, 1000}, { 11, 1700}, { 17, 2500} };
        bool isClear = true;
        Player player = GameManager.I().GetPlayer();
        ViewTitle title = ViewTitle.던전결과;
        Random random = new Random();

        //보상 계산식
        int totalDef = player.Defence + player.AdditionalDefence;
        int totalAtk = player.Attack + player.AdditionalAttack;
        int lostHealth = random.Next(20, 36) - totalDef + dungeonBalance[dungeonLv - 1, 0];
        int rewardGold = dungeonBalance[dungeonLv - 1, 1] + (int)(dungeonBalance[dungeonLv - 1, 1] * (totalAtk * (random.NextDouble() + 1) / 100 + 1));
        
        if (player.Defence + player.AdditionalDefence < dungeonBalance[dungeonLv - 1, 0]) //권장방어력보다 낮다면
        {
            if (random.NextDouble() < 0.4) { lostHealth /= 2; rewardGold = 0; isClear = false; }
        }

        //View부분
        ViewUtil.PrintTitle(title);
        if (isClear) Console.WriteLine($"축하합니다!!\n{doungeonName[dungeonLv - 1]}을 클리어 하셨습니다.\n");
        else Console.WriteLine($"안타깝게도\n{doungeonName[dungeonLv - 1]}을 실패했습니다.\n");
        Console.WriteLine("[탐험 결과]");
        Console.WriteLine($"체력 {player.Health}->{player.Health-lostHealth}");
        Console.WriteLine($"Gold {player.Gold} G -> {player.Gold + rewardGold} G\n");

        //계산 적용 및 나가기
        player.Health -= lostHealth;
        player.Gold += rewardGold;
        if (isClear) player.AddExp();
        ViewUtil.GetUserInput(0, 0);//마을가기 입력받기
    }
    static void RestView()
    {
        int cost = 500;
        Player player = GameManager.I().GetPlayer();
        ViewTitle title = ViewTitle.휴식하기;
        string[] selection = ["휴식하기"];

        ViewUtil.PrintTitle(title);
        Console.WriteLine($"{cost} G를 내면 체력을 회복할 수 있습니다. (보유 골드: {player.Gold} G)");
        ViewUtil.PrintSelection(selection);
        int userInput =ViewUtil.GetUserInput(0, selection.Length);
        if (userInput == 1 && player.Gold >= cost)
        {
            player.Health = 100;
            player.Gold -= 500;
            Console.WriteLine("휴식을 완료했습니다.");    
        }
        else if(userInput == 1 && player.Gold < cost)
        {
            Console.WriteLine("Gold가 부족합니다."); 
        }
        Thread.Sleep(800);
        CallView(title);
    }

    static void Main(string[] args)
    {
        GameManager GM = GameManager.I();
        if (File.Exists(filePath)) SelectLoadView();
        GM.GameStart();
    }
    public class GameManager
    {
        private static GameManager gameManager;
        public static GameManager I()//싱글톤 패턴
        {
            if (gameManager == null)
            {
                gameManager = new GameManager();
            }
            return gameManager;
        }
        public Player player;
        public bool isLoadGame = false;
        public void GameStart()
        {
            //View 2개를 거쳐 리턴값으로 플레이어 초기화
            if (!isLoadGame) player = new Player(ReceiveNameView(), SelectJobView());            
            else player = Player.LoadFromJson(filePath); //로드된 플레이어 
            CallView(ViewTitle.마을);
        }
        public Player GetPlayer() {  return player; }
    }
    
    static string filePath = "./Save/SaveFile.json";
    
    internal enum JobType { 전사, 도적}
}

