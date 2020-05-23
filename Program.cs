using System;
using System.Collections.Generic;
using static System.Console;
using System.Reflection;

namespace Legends_Legend
{
    //----------------------------------------------------
    //--------------- Сборники констант ------------------
    //----------------------------------------------------
    public interface IDamagesTypes
    {
        protected internal const int TypesCount = 3;
        public const int Melee = 0;
        public const int Archery = 1;
        public const int Magic = 2;
    }
    public interface IMissionTypes
    {
        protected internal const int TypesCount = 3;
        public const int Bring = 0;
        public const int Kill = 1;
        public const int Talk = 2;
    }

    //----------------------------------------------------
    //------- Classes for objects and creatures ----------
    //----------------------------------------------------
    public class Creature 
    {
        //------------ Личные параметры перса ------------- <Creature>
        public string Name;
        public string LifeForm;
        public int maxHP_internal;
        private protected int HP_internal;
        public int HP
        {
            set { HP_internal = (value > 0) ? value : 0; if (HP == 0) WriteLine($"{LifeForm} {Name} is dead."); if (HP_internal > maxHP) HP_internal = maxHP; }
            get { return HP_internal; }
        }
        public int maxHP
        {
            set { maxHP_internal = (value > 0) ? value : 1; }
            get { return maxHP_internal; }
        }

        public int[] Atack = new int[IDamagesTypes.TypesCount];
        public int[] Defence = new int[IDamagesTypes.TypesCount];

        // ---- Instruments for ID ----
        static int Objects_Count = 0;
        public int ID;

        public List<int> Mission_Connection = new List<int>(0); //Связи с миссиями
        public List<Phys_Object> pocket = new List<Phys_Object> { };//inventory

        //------------ Constructors --------------- <Creature>
        public Creature() : this("Тигр") { }
        public Creature(string Form_Of_Life)
        {
            LifeForm = Form_Of_Life;
            maxHP = HP = 100;
            ID = Objects_Count++;
        }
        public Creature(string Form_Of_Life, int Hit_Points)
        {
            LifeForm = Form_Of_Life;
            maxHP = HP = Hit_Points;
            maxHP = HP = 100;
            ID = Objects_Count++;
        }
        public Creature(string Form_Of_Life, string Creature_Name, int Hit_Points):this(Form_Of_Life, Hit_Points)
        {
            Name = Creature_Name;
        }

        //-------------- Mothods -----------------
        public void Set_Atack_Parameters(int[] Atack_Set)
        {
            int i;
            if (Atack_Set.Length >= IDamagesTypes.TypesCount)
                for (i = 0; i < IDamagesTypes.TypesCount; i++)
                    Atack[i] = Atack_Set[i];
            else
            {
                for (i = 0; i < Atack_Set.Length; i++)
                    Atack[i] = Atack_Set[i];
                for (i = Atack_Set.Length; i < IDamagesTypes.TypesCount; i++)
                    Atack[i] = 0;
            }
        }
        public void Set_Deffence_Parameters(int[] Defence_Set)
        {
            int i;
            if (Defence_Set.Length >= IDamagesTypes.TypesCount)
                for (i = 0; i < IDamagesTypes.TypesCount; i++)
                    Defence[i] = Defence_Set[i];
            else
            {
                for (i = 0; i < Defence_Set.Length; i++)
                    Defence[i] = Defence_Set[i];
                for (i = Defence_Set.Length; i < IDamagesTypes.TypesCount; i++)
                    Defence[i] = 0;
            }
        }


        //------------- Comparing ----------------
        public override string ToString()
        { return $"{LifeForm}: {(Name?? "unnamed")} with {HP} HP, ID: {ID}"; }
    }

    public class NPC:Creature
    {
        //------------ Variables & Constants ------------- <NPC>
        public string profession = "Фермер";
        private int Purce_internal = 0;
        public int purce
        {
            set{ Purce_internal = (value >= 0) ? value : 0; }
            get { return Purce_internal; }
        }
                                                                    
        //------------ Constructors --------------- <NPC>
        public NPC() : base("Человек")
        {
            Name = "Путешественник";
            maxHP = 10;
        }
        public NPC(string NPC_Name) : base("Человек")
        {
            Name = NPC_Name;
        }
        public NPC(string NPC_Name,int hitpoints) : base("Человек", NPC_Name, hitpoints)
        {
        }
    }

    public class Phys_Object
    {
        public string name;
        public int price;

        public bool Mission_Deals = false; //Mission connection, starts checking when changed
        public Phys_Object():this("Мусор")
        {
            price = 1;
        }
        public Phys_Object(string this_object)
        {
            name = this_object;
            price = 5;
        }
        
        public Phys_Object(string object_is, int object_price)
        {
            name = object_is;
            price = (object_price >= 0) ? object_price :1;
        }

        public Phys_Object(Phys_Object cr)
        {
            this.price = cr.price;
            this.name = cr.name;
            this.Mission_Deals = cr.Mission_Deals;
        }
        //------------- Comparing ----------------
        public override string ToString() { return $"{(name ?? "unnamed")} стоимостью: {price}"; }
        public static bool operator == (Phys_Object A, Phys_Object B)
        {
            return (A.name == B.name) & (A.price == B.price);
        }
        public static bool operator !=(Phys_Object A, Phys_Object B)
        {
            return !((A.name == B.name) & (A.price == B.price));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public class Stuff_Container
    {
        public string Name;
        public bool IS_Locked;
        public List<Phys_Object> Items = new List<Phys_Object>(0);
        public Stuff_Container()
        {
            Name = "Старый сундук";
            IS_Locked = false;
        }
        public Stuff_Container(string Chest_Name)
        {
            Name = Chest_Name;
            IS_Locked = false;
        }
    }

    //----------------------------------------------------
    //----------------- Work structures ------------------
    //----------------------------------------------------
    struct Stuff_Juggler//Transfering things
    {
        //From List to Creature
        private bool MoveItem(ref List<Phys_Object> Where_Takes, ref List<Phys_Object> Who_Takes, int what_takes)
        {
            if (Where_Takes.Count <= (what_takes) | what_takes < 0)
            {
                WriteLine($"There is no #{what_takes} things! Only #{Where_Takes.Count - 1}");
                return false;
            }
            else
            {
                Who_Takes.Add(Where_Takes[what_takes]);
                Where_Takes.RemoveAt(what_takes);
                WriteLine($"Got it!");
                return true;
            }

        }
        public bool Transfer(Stuff_Container Where_Takes, Creature Who_Takes, int what_takes)
        {
            return MoveItem(ref Where_Takes.Items, ref Who_Takes.pocket, what_takes);
        }
        public bool Transfer(Creature Where_Takes, Stuff_Container Who_Takes, int what_takes)
        {
            return MoveItem(ref Where_Takes.pocket, ref Who_Takes.Items, what_takes);
        }
        public bool Transfer(Creature Where_Takes, Creature Who_Takes, int what_takes)
        {
            return MoveItem(ref Where_Takes.pocket, ref Who_Takes.pocket, what_takes);
        }
    }

    struct Condition_Checker//Check mission triggers 
    {
        public bool Item_Achieved(Phys_Object Item, Creature Who_Takes)//Do he have this thing??
        {
            for (int i = 0; i < Who_Takes.pocket.Count; i++)
                if (Who_Takes.pocket[i] == Item) //run equality check by == in Phys_Object
                {
                    WriteLine($"Place {i}");
                    return true;
                }
            return false;
        }

        public bool Creature_Is_Dead(Creature Who_Must_Die)//chek who must be killed!
        {
            return Who_Must_Die.HP == 0; //if hp == 0 then he dead
        }

        public bool Spoken_With(Creature Who_Must_Speak)//when you must speak with someone
        {
            return true;//??????
        }
    }

    struct Display_Parametrs//Display lists and parametrs
    {
        public void Stuff(Creature cr)//Display items of Creature
        {
            Console.ForegroundColor = ConsoleColor.Green;
            string who_is = (cr.LifeForm == "Человек") ? cr.Name : cr.LifeForm;
            if (cr.pocket.Count == 0) WriteLine($">>{who_is} ничего не имеет за душой!");
            else
            {
                WriteLine($">>{who_is} обладает кое чем:");
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (Phys_Object whoa in cr.pocket)
                { WriteLine(" - " + whoa); }
            }
            Console.ResetColor();
            WriteLine();
        }
        public void Creatures_List(List<Creature> cr)//Display all heroes and monsters of Creature
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            WriteLine(">>Все герои");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            for (int i = 0; i < cr.Count; i++)
            { WriteLine(cr[i] + $", ID: {cr[i].ID}"); }
            Console.ResetColor();
            WriteLine();
        }
        public void Stuff(Stuff_Container obj)//Display items of Creature
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            if (obj.Items.Count < 1) WriteLine($">>{obj.Name} абсолютно пуст!");
            else
            {
                WriteLine($">>{obj.Name} хранит:");
                Console.ForegroundColor = ConsoleColor.Cyan;
                foreach (Phys_Object whoa in obj.Items)
                { WriteLine(" - " + whoa); }
            }
            Console.ResetColor();
            WriteLine();
        }
        public void Display_Atack(Creature cr)//Display atakc parametrs
        {
            string who_is = (cr.LifeForm == "Человек") ? cr.Name : cr.LifeForm;
            Write($"{who_is} обладает следующими параметрами атаки:");
            for (int i = 0; i < IDamagesTypes.TypesCount; i++)
                Write($" [{cr.Atack[i]}]");
            WriteLine();
        }
    }


    //----------------------------------------------------
    //------------------ Speak system --------------------
    //----------------------------------------------------

    public class Thema //Одна тема беседы, одна линейка разговора
    {
        // --------- Variables and constants ---------------
        public string Title;//Название темы или миссии
        public List<(bool, string)> Phrase_inter = new List<(bool, string)>(0);//кортежи одной фразы одного человека, тру - мы говорим
        public int bout_mission_num = -1;
        private int _area = 1;
        public int Area // участок в листе, отвечающий требованиям 1, 2 или 3й
        {set { if (value > 0 & value <= 4) _area = value; } get { return _area; }  }
        public int[] AreaStartPoint = new int[] { 0, 0, 0, 0 ,0 }; // где начинается участок

        public (bool,string) this[int i] // модификатор доступа объект как массив [], с учётом рабочего участка
            { 
                //set { Phrase_inter[ AreaStartPoint[Area - 1] + i ] = value; }
                get { return Phrase_inter[ AreaStartPoint[Area - 1] + i ]; }
            }

        public int Count
        { get { return AreaStartPoint[Area] - AreaStartPoint[Area-1]; } }

        // Key words for dialog:
        static public string[] Key_Words = new string[] {

            "ATACK",            // when need to fight
            "CHECK",            // start cheking missions
            "END_DIALOG",       // marks dialog on ending
            "MISSION_NAME",     //  CHANGE on misiion name
            "NPC_PROFESSION",   // CHANGE on NPC.Name
            "NPC_NAME",         // CHANGE on NPC.Name
        };
        public Thema(string Thema_Name)//Конструктор, просматривает общий список выбирает одну тему
        {
            Title = Thema_Name;
            for (int i = 0; i < Phrase_Base.Count; i++)
            {
                if (Phrase_Base[i].Item1 == Thema_Name)
                {
                    AreaStartPoint[Phrase_Base[i].Item2-1] ++; // сперва получаем длины участков
                    Phrase_inter.Add((Phrase_Base[i].Item3, Phrase_Base[i].Item4));
                }
            }
            // теперь из длин делаем начала участков
            AreaStartPoint[4] = AreaStartPoint[0] + AreaStartPoint[1] + AreaStartPoint[2] + AreaStartPoint[3];
            AreaStartPoint[3] = AreaStartPoint[0] + AreaStartPoint[1] + AreaStartPoint[2];
            AreaStartPoint[2] = AreaStartPoint[0] + AreaStartPoint[1];
            AreaStartPoint[1] = AreaStartPoint[0];
            AreaStartPoint[0] = 0;
        }

        private static readonly List<(string, int, bool, string)> Phrase_Base = new List<(string, int, bool, string)>//База всех фраз и миссий(название, говорит ГГ, сама фраза
        {
            /* 1 - без миссии
             * 2 - Выдача миссии
             * 3 - Миссия выдана, нпс ждёт
             * 4 - Миссия выполнена, нпс кайфует */

            ( "Приветствие", 1, true, "Эй, NPC_NAME, как дела?" ),
            ( "Приветствие", 1, false, "Работа, чего тебе надо?" ),
            ( "Приветствие", 1, true, "Прост побазаить немношк" ),
            ( "Приветствие", 1, false, "Ой, я NPC_PROFESSION... мне не до разговоров!" ),
            ( "Приветствие", 1, false, "мне не бдо разговоров!" ),
            ( "Приветствие", 1, true, "Ок" ),
            ( "Приветствие", 1, false, "Итак" ),

            ( "Приветствие", 2, true, "Эй, NPC_NAME, как дела?" ),
            ( "Приветствие", 2, false, "Ох, знаешь, беда" ),
            ( "Приветствие", 2, true, "Что такое, дружище?" ),
            ( "Приветствие", 2, false, "До жути хочу MISSION_ITEM" ),
            ( "Приветствие", 2, true, "Так я тебе приволоку!" ),
            ( "Приветствие", 2, false, "Серьёзн? ты клёвый посан :'))" ),
            ( "Приветствие", 2, true, "А то 8)" ),

            ( "Приветствие", 3, false, " Что там с моей просьбой?" ),
            ( "Приветствие", 3, true, "Ах, это..." ),
            ( "Приветствие", 3, false, "Да, ты хотел принести мне MISSION_ITEM" ),
            ( "Приветствие", 3, true, "Как раз собирался за ней!" ),
            ( "Приветствие", 3, false, "Не знаю долго ли я продержусь без него... MISSION_ITEM моё спасение!" ),

            ( "Приветствие", 4, false, " Рад тебя видеть, мой герой!" ),
            ( "Приветствие", 4, true, "Ну что, MISSION_ITEM тебе по нраву?" ),
            ( "Приветствие", 4, false, "Вообще супер! Спасибо тебе!" ),
            
            // Техническая тема, прощание
            ( "Прощание", 1, true, "Ну, пока! END_DIALOG" ),

            /* 1 - Диалог с прочими - не в курсе дел
             * 2 - Диалог с выдавшим - ждёт
             * 3 - Диалог с выдавшим - принимает сдачу миссии
             * 4 - Диалог с прочими - связанный с миссией */

            ( "BRING", 1, true, "Что ты знаешь по поводу MISSION_ITEMа?" ),
            ( "BRING", 1, true, "Не знаю ничего"),
            ( "BRING", 1, true, "Спасибо!"),
            ( "BRING", 1, false, "Что то ещё?"),

            ( "BRING", 2, true, "Есть что то ещё по поводу MISSION_ITEMа?"),
            ( "BRING", 2, false, "Ты мне скажи!"),
            ( "BRING", 2, true, "Я как раз собирался идти за этой штукенцией"),
            ( "BRING", 2, false, "Пасябки"),

            ( "BRING", 3, true, "Мужыыык, смари чо!" ),
            ( "BRING", 3, true, "Чо?"),
            ( "BRING", 3, true, "MISSION_ITEM"),
            ( "BRING", 3, false, "Ядрёна Матрёна, фигаси!!"),
            ( "BRING", 3, true, "8)"),
            ( "BRING", 3, false, "Ты клёвый посан!"),

            ( "BRING", 4, true, "Что ты знаешь по поводу MISSION_ITEMа?" ),
            ( "BRING", 4, true, "Знаю что то"),
            ( "BRING", 4, true, "Спасибо!"),
            ( "BRING", 4, false, "Что то ещё?"),
              

        };
    }

    class Conversation_Machine
    {
        delegate void Action(); // Запись действий при выборе какой то строки
        public List<Thema> Dialog_Theme_List = new List<Thema>(0); // Общий список тем ГГ

        public Conversation_Machine()
        {
            Add_Theme("Приветствие", -1);
            Add_Theme("Прощание", -1);
        }
        public bool Add_Theme(string New_Theme, int Mission_Connect)
        {
            foreach (Thema th in Dialog_Theme_List)
            {
                if (th.Title == New_Theme) return false;
            }
            Dialog_Theme_List.Add(new Thema(New_Theme) { bout_mission_num = Mission_Connect});
            if (Dialog_Theme_List[Dialog_Theme_List.Count - 1].Count > 0) return true;
            else
            {
                Dialog_Theme_List.RemoveAt(Dialog_Theme_List.Count - 1);
                return false;
            }

        }

        public bool Remove_Theme(int Mission_Complete_ID)
        {
            for ( int i = 0; i< Dialog_Theme_List.Count ; i++ )
            {
                if (Dialog_Theme_List[i].bout_mission_num == Mission_Complete_ID)
                { Dialog_Theme_List.RemoveAt(i); return true; }
            }
            return true;
        }
        
        // ---------------- Dialog start with someone --------------
        public void Speak(NPC Humanoid, Mission_Engine Mission_Background)
        {
            // ------------------------------------ Variables -------------------------------------
            int Step; //coursor in chosen theme
            string NPC_Phrase = "Здравствуй, меня зовут NPC_NAME. "; // NPC text
            List<string> Dialog_Case_Out = new List<string>(0);//strings for output 
            Action Output_Action = null;// Действия при выборе из списка
            List<Action> Dialog_Case_Action = new List<Action>(0);// действия для выбранного пункта диалога
            bool Dialog_Ended = false; // When must go out dialog

            // Метод ------------ Заполняем ключи в строке ----------------------------------------
            string Apply_Phrase(string pr_Phrase)
            {
                Output_Action = null;
                string end_string = pr_Phrase;
                int found_on;
                foreach (string key in Thema.Key_Words)
                {
                    found_on = end_string.IndexOf(key);
                    if (found_on >= 0)
                    {
                        end_string = end_string.Remove(found_on, key.Length);
                        // -------------------------- Actions on keyword!! -----------------------------
                        switch (key)
                        {
                            case "NPC_NAME":
                                end_string = end_string.Insert(found_on, Humanoid.Name);
                                break;
                            case "NPC_PROFESSION":
                                end_string = end_string.Insert(found_on, Humanoid.profession);
                                break;
                            case "END_DIALOG": Output_Action += delegate { Dialog_Ended = true; }; break;
                            case "CHECK": break;
                            case "ASKING_MISSIONS": break;
                            case "IF_NOT_FAMILIAR": break;
                        }
                    }
                }
                return end_string;
            }
            // Метод ------------ Разговор о какой то конкретной теме -----------------------------
            void Talk_About(Thema Theme)
            {
                Theme_Step(Theme);
                string Talk_About = "";
                while (Talk_About != "END")
                {
                    List<string> Dialog_Case_Out = new List<string>(0);//strings for output 
                    List<Action> Dialog_Case_Action = new List<Action>(0);// действия для выбранного пункта диалога
                    NPC_Phrase = ""; //Clean output
                    Talk_About = Theme_Step(Theme);
                    if (Talk_About == "END") break;
                    Dialog_Case_Out.Add(Apply_Phrase(Talk_About));
                    Dialog_Case_Action.Add(Output_Action);
                    Shog_Dialog_Window(Apply_Phrase(NPC_Phrase), Dialog_Case_Out);
                    int choose = 0;
                    while (choose == 0)
                    {
                        string Enter = ReadLine();
                        if (!Int32.TryParse(Enter, out choose)) WriteLine("Not valid nember of string dialog");
                        else if (choose < 1 || choose >= Dialog_Theme_List.Count)
                        { choose = 0; WriteLine("Not valid nember of string dialog"); }
                        
                    };

                }

            }
            // Метод ------------ Следующий шаг в теме, если следующего нет, но return false, My_Words = "END"
            string Theme_Step(Thema My_Theme)
            {
                if (Step + 1 == My_Theme.Count)//Если следующей фразы нет, тема сворачиванется
                    return "END";

                Step++;

                if (My_Theme[Step].Item1) { return My_Theme[Step].Item2; } // если true - наша фраза, иначе собираем
                else
                {
                    while (!My_Theme[Step].Item1) // собираем все не мои фразы, проверяем есть ли ещё
                    {
                        NPC_Phrase += My_Theme[Step].Item2;
                        if (Step + 1 == My_Theme.Count)//Если следующей фразы нет, тема сворачиванется
                            return "END";
                        Step++;
                    }
                    return (My_Theme[Step].Item2); //снова добрались до моей фразы
                }
            }
            // Метод------------ Выводит заголовок НПС и варианты ответа
            void Shog_Dialog_Window(string His_Words, List<string> My_words)
            {
                int i;
                BackgroundColor = ConsoleColor.Blue;
                Write(His_Words);
                ResetColor(); WriteLine();

                for (i = 0; i < My_words.Count; i++)
                {
                    BackgroundColor = ConsoleColor.Green;
                    Write((i + 1) + ". " + My_words[i]);
                    ResetColor(); WriteLine();
                }
                BackgroundColor = ConsoleColor.Green;
                ResetColor(); WriteLine(Dialog_Case_Out.Count + " " + Dialog_Case_Action.Count) ;
            }
            // Метод ----------- Выбирает вариант диалога в зависимости от обстоятельств ----------
            Action Dailog_Area(Thema Look_In)
            {
                Look_In.Area = 1;
                if (Look_In.Title == "Приветствие" & Humanoid.Mission_Connection.Count > 0)
                {
                    for (int i = 0; i< Humanoid.Mission_Connection.Count; i++)
                    {
                        if (Mission_Background[i].Available & (!Mission_Background[i].Given) & (Mission_Background[i].Whos_Target == Humanoid.ID) & (!Mission_Background[i].Rewarded))
                        {
                            Look_In.Area = 2;
                            return delegate { 
                                Mission_Background[i].Given = true;
                                Add_Theme("BRING", i);
                            }; 
                        }
                        if  (Mission_Background[i].Available & (Mission_Background[i].Given) & (Mission_Background[i].Whos_Target == Humanoid.ID) & (!Mission_Background[i].Rewarded))
                        { Look_In.Area = 3; return null; }
                        if (Mission_Background[i].Rewarded)
                        { Look_In.Area = 4; }
                    }
                }
                
                if (Look_In.Title == "BRING" & Humanoid.Mission_Connection.Contains(Look_In.bout_mission_num))
                {
                    Mission_Background[Look_In.bout_mission_num].Check();
                    if (Mission_Background[Look_In.bout_mission_num].Done)
                    { 
                        Look_In.Area = 3;
                        return delegate 
                        { Mission_Background[Look_In.bout_mission_num].Rewarded = true;
                            Remove_Theme(Look_In.bout_mission_num);
                        };
                    } else
                    { Look_In.Area = 2; return null; }
                }
                return null;
            };


            while (!Dialog_Ended)
            {
                // -------------------------- Начало дивлога, построение первонаяального списка  -------------------------
                Dialog_Case_Out.Clear(); Dialog_Case_Action.Clear();
                for (int i = 0; i < Dialog_Theme_List.Count; i++)//Перебор по одной фразе
                {
                    Step = -1;
                    Action Miss_Con = Dailog_Area(Dialog_Theme_List[i]);
                    Dialog_Case_Out.Add(Apply_Phrase(Theme_Step(Dialog_Theme_List[i])));
                    Dialog_Case_Action.Add(Output_Action + Miss_Con);
                }
                Shog_Dialog_Window(NPC_Phrase, Dialog_Case_Out);

                // --------------------------- Выбор номера темы или выхода ---------------------------------
                int choose = 0;
                while (choose == 0)
                {
                    string Enter = ReadLine();
                    if (!Int32.TryParse(Enter, out choose)) WriteLine("Not valid nember of string dialog");
                    else if (choose < 1 || choose > Dialog_Theme_List.Count)
                    { choose = 0; WriteLine("Not valid nember of string dialog"); }
                };

                // --------------------------- Чтение номера варинта, выход, либо начало продвижения по теме --------
                Step = -1;
                
                Talk_About(Dialog_Theme_List[choose - 1]); //в вариантах с 1 а в массиве с 0
                Dialog_Case_Action[choose - 1]?.Invoke(); // Запуск действия при выборе, если есть
            }
            // ------ Из диалога вышли, оплучен сигнал о выходе
            WriteLine("Bye");


        }
    }
    
    //----------------------------------------------------
    //---------- Missions engine structures --------------
    //----------------------------------------------------

    class Mission_Engine
    {
        // --------- Single acts ------------
        delegate bool ConMission();
        public class Acts
        {

            //PARAMETRS OF SINGLE ACT
            public string Title;    // название миссии
            public string Description; // описание, что нужно сделать

            public bool Available;  // доступность миссии для получения
            public bool Given;      // выдана ли миссия
            public bool Done;       // сделано ли условие миссии
            public bool Rewarded;   // получена ли награда (миссия закрыта)
            ConMission Mission;     // проверка условия выполненности миссии

            public int Who_Gives;
            public int Whos_Target;
            static public Condition_Checker Conditions = new Condition_Checker();

            // ----- for ID ---------
            static int count_acts = 0;
            public int ID;
            // Конструктор миссии принеси-подай
            public Acts(Phys_Object Find, Creature Finder)
            {
                Available = true;
                Given = Done = Rewarded = false;
                Description = (Finder.LifeForm == "Человек") ? Finder.Name : Finder.LifeForm;
                Description += " должен иметь: " + Find.name + " стоимостью " + Find.price;
                Title = Find.name;
                Mission = delegate { return Conditions.Item_Achieved(Find, Finder); };
                Who_Gives = Finder.ID; Whos_Target = Finder.ID;
                ID = count_acts++;
            }

            // Конструктор миссии об убийстве
            public Acts(Creature Condemned)
            {
                Given = Done = Rewarded = false;
                Description = ((Condemned.LifeForm == "Человек") ? Condemned.Name : Condemned.LifeForm) + " должен умереть!";
                Mission = delegate { return Conditions.Creature_Is_Dead(Condemned); };
            }
            //check condition
            public void Check()
            {
                bool If = Mission();
                if (If) Done = true;
            }
        }

        public List<Acts> Mission_Acts = new List<Acts>(0);

        public Acts this[int i] // модификатор доступа объект как массив [], с учётом рабочего участка
        {
            //set { Phrase_inter[ AreaStartPoint[Area - 1] + i ] = value; }
            get { return Mission_Acts[i]; }
        }

        //Constructor by mission count
        public Mission_Engine()
        { }
        public Mission_Engine(Stuff_Container Items, int count_brings, List<Creature> Cr, int count_kills)
        {
            int obj;
            int hum;
            Random rnd = new Random();
            for (int i = 0; i < count_brings; i++) // добавляем действия принеси- подай, i ещё и номер добавляемой миссии
            {
                obj = rnd.Next(Items.Items.Count);
                hum = rnd.Next(Cr.Count);
                Mission_Acts.Add(new Acts(Items.Items[obj], Cr[hum]));
                Cr[hum].Mission_Connection.Add(i);
            }
            for (int i = count_brings; i < count_brings + count_kills; i++) //добавляем миссии убей-замочи (не сделано!)
            {
                hum = rnd.Next(Cr.Count);
                Mission_Acts.Add(new Acts(Cr[hum]));
            }

        }

        public void Add_Bringer_Mission(Phys_Object Item, Creature Cr)
        {
            Mission_Acts.Add(new Acts(Item, Cr));
            Cr.Mission_Connection.Add(Mission_Acts.Count -1);
        }
        public void All_Missions()
        {
            WriteLine("All missions");
            for (int i = 0; i < Mission_Acts.Count; i++)
                WriteLine($"{i}) {Mission_Acts[i].Description}. Done: {Mission_Acts[i].Done}, ID: {Mission_Acts[i].ID}");
        }
        public void Check_Missions()
        {
            for (int i = 0; i < Mission_Acts.Count; i++)
                Mission_Acts[i].Check();
        }

    }

    //----------------------------------------------------
    //--------====-------- Program --------====-----------
    //----------------------------------------------------
    class Program
    {
        static void Main(string[] args)
        {
            //-----------------------------
            // Создание рабочих экзе-ров
            //-----------------------------
            Display_Parametrs       Display = new Display_Parametrs();          //Экзе-р для вывода списков
            Stuff_Juggler           Stuff = new Stuff_Juggler();                //Экзе-р для перемещений предметов
            Conversation_Machine    Conversation = new Conversation_Machine();  //Экзе-р для диалогов

            //-----------------------------
            //Initialize heroes
            //-----------------------------
            List<Creature> Peoples = new List<Creature> { new NPC("ГГ", 150), new NPC("Алгор воитель", 500),new NPC() };
            //Peoples.AddRange(new Creature[] { new Creature(), new NPC(), new Creature(), new NPC() }) ;
            Display.Creatures_List(Peoples);

            //-----------------------------
            //Initialize objects
            //-----------------------------
            Stuff_Container Chest = new Stuff_Container("Сундучара");
            Chest.Items.AddRange(new List<Phys_Object>
            {new Phys_Object(), new Phys_Object(), new Phys_Object("Секира") { price = 10 } , new Phys_Object("Амулет", 100) });
            Display.Stuff(Chest);

            //-----------------------------
            //Initialize missions
            //-----------------------------
            Mission_Engine My_Story = new Mission_Engine();// (Chest, 1, Peoples, 0); // Экзе-р с миссиями по имеющимся людям и вещам
            My_Story.All_Missions();

            Stuff.Transfer(Chest, Peoples[0], 3);
            Peoples[1].HP = 0;

            Display.Stuff(Chest);
            for (int i = 0; i < Peoples.Count; i++)
                Display.Stuff(Peoples[i]);

            My_Story.Check_Missions();
            My_Story.All_Missions();


            My_Story.Add_Bringer_Mission(Chest.Items[0], Peoples[1]);
            My_Story.All_Missions();

            Conversation.Speak(Peoples[1] as NPC, My_Story);

            Stuff.Transfer(Chest, Peoples[1], 0);

            Conversation.Speak(Peoples[1] as NPC, My_Story);
        }
    }

}
