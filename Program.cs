using System;
using System.Collections.Generic;
using static System.Console;
using System.Reflection;

namespace Legends_Legend
{
    //--------------------------------------------------------------------------------------
    //----------------- Classes for objects and creatures ----------------------------------
    //--------------------------------------------------------------------------------------
    interface IDamagesTypes
    {
        protected internal const int TypesCount = 3;
        public const int Melee = 0;
        public const int Archery = 1;
        public const int Magic = 2;
    }

    class Creature
    {
        //------------ Личные параметры перса ------------- <Creature>
        public string Name;
        public string LifeForm;
        public int maxHP_internal;
        private protected int HP_internal = 10;
        public int HP
        {
            set { HP_internal = (value >= 0) ? value : 0; if (HP == 0) WriteLine($"{LifeForm} {Name} is dead."); if (HP_internal > MaxHP) HP_internal = MaxHP; }
            get { return HP_internal; }
        }
        public int MaxHP
        {
            set { maxHP_internal = (value > 0) ? value : 1; }
            get { return maxHP_internal; }
        }

        public int[] Atack = new int[IDamagesTypes.TypesCount];
        public int[] Defence = new int[IDamagesTypes.TypesCount];

        // ---- Instruments for ID ----
        static int Objects_Count = 0;
        public int ID;

        //public List<int> Mission_Connection = new List<int>(0); //Связи с миссиями
        public List<Phys_Object> pocket = new List<Phys_Object> { };//inventory

        //------------ Constructors --------------- <Creature>
        public Creature() : this("Тигр") { }
        public Creature(string Form_Of_Life)
        {
            LifeForm = Form_Of_Life;
            ID = Objects_Count++;
        }
        public Creature(string Form_Of_Life, int Hit_Points)
        {
            LifeForm = Form_Of_Life;
            MaxHP = Hit_Points;
            HP = Hit_Points;
            ID = Objects_Count++;
        }
        public Creature(string Form_Of_Life, string Creature_Name, int Hit_Points) : this(Form_Of_Life, Hit_Points)
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
        public int ItemIndex(Phys_Object Find)
        {
            for (int i = 0; i < pocket.Count; i++)
                if (pocket[i].name == Find.name & pocket[i].price == Find.price)
                    return i;
            return -1;
        }
        public override string ToString()
        { return $"{LifeForm}: {(Name ?? "unnamed")} with {HP} HP, ID: {ID}"; }
    }

    class NPC : Creature
    {
        //------------ Variables & Constants ------------- <NPC>
        public string profession = "Фермер";
        private int _purce = 0;
        public int Purce
        {
            set { _purce = (value >= 0) ? value : 0; }
            get { return _purce; }
        }

        //------------ Constructors --------------- <NPC>
        public NPC() : base("Человек")
        {
            Name = "Путешественник";
            MaxHP = 10;
            HP = 10;
        }
        public NPC(string NPC_Name) : base("Человек")
        {
            Name = NPC_Name;
        }
        public NPC(string NPC_Name, int hitpoints) : base("Человек", NPC_Name, hitpoints)
        {
        }
    }

    public class Phys_Object
    {
        public string name;
        public int price;

        public Phys_Object() : this("Мусор")
        {
            price = 1;
        }
        public Phys_Object(string Object_Name)
        {
            name = Object_Name;
            price = 1;
        }
        public Phys_Object(string Object_Name, int Object_Price)
        {
            name = Object_Name;
            price = (Object_Price >= 0) ? Object_Price : 1;
        }
        public Phys_Object(Phys_Object CopiedObject)
        {
            this.price = CopiedObject.price;
            this.name = CopiedObject.name;
        }
        //------------- Comparing ----------------

        public override string ToString() { return $"{(name ?? "unnamed")} стоимостью: {price}"; }
        public static bool operator ==(Phys_Object A, Phys_Object B)
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

    class Stuff_Container
    {
        public string Name;
        public bool IS_Locked = false;
        public List<Phys_Object> Items = new List<Phys_Object>(0);
        public Stuff_Container() : this("Старый сундук")
        {
        }
        public Stuff_Container(string Chest_Name)
        {
            Name = Chest_Name;
        }
    }

    //--------------------------------------------------------------------------------------
    //------------------- Work structures --------------------------------------------------
    //--------------------------------------------------------------------------------------
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
                    //WriteLine($"Place {i}");
                    return true;
                }
            return false;
        }

        public bool Creature_Is_Dead(Creature Who_Must_Die)//chek who must be killed!
        {
            return Who_Must_Die.HP == 0; //if hp == 0 then he dead
        }

        //public bool Spoken_With(Creature Who_Must_Speak)//when you must speak with someone
        //{
        //  return true;//??????
        //}
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
            { WriteLine(cr[i] + $", ID: {cr[i].ID}, MaxHP: {cr[i].MaxHP}, HP: {cr[i].HP}"); }
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


    //--------------------------------------------------------------------------------------
    //------------------ Speak & Mission system --------------------------------------------
    //--------------------------------------------------------------------------------------

    public class Thema //Одна тема беседы, одна линейка разговора
    {
        // --------- Variables and constants ---------------
        public string Title;//Название темы или миссии
        private readonly List<(bool, string)> Phrase_inter = new List<(bool, string)>(0);//кортежи одной фразы одного человека, тру - мы говорим
        public int bout_mission_num = -1;
        private int _area = 1;
        public int Area // участок в листе, отвечающий требованиям 1, 2 или 3й
        { set { if (value > 0 & value <= 4) _area = value; } get { return _area; } }
        public int[] AreaStartPoint = new int[] { 0, 0, 0, 0, 0 }; // где начинается участок

        public (bool, string) this[int i] // модификатор доступа объект как массив [], с учётом рабочего участка
        { get { return Phrase_inter[AreaStartPoint[Area - 1] + i]; } }

        public int Count
        { get { return AreaStartPoint[Area] - AreaStartPoint[Area - 1]; } }

        // Key words for dialog:
        public static readonly string[] Key_Words = new string[] {

            "ATACK",            // when need to fight
            "END_DIALOG",       // marks dialog on ending
            "MISSION_COMPLETE", // Выполнение миссии
            "MISSION_INFORMATION", //Информация по прохождению необходимая для миссии
            "MISSION_ITEM",     // ПОМЕНЯТЬ на объект миссии
            "MISSION_GIVE",     // Выдать миссию
            "MISSION_NAME",     // CHANGE on mission name
            "MISSION_PRICE",    // Цена получения объекта, возможно миссии
            "MISSION_TALK",     // Засчитывается разговор в миссии
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
                    AreaStartPoint[Phrase_Base[i].Item2 - 1]++; // сперва получаем длины участков
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
            /* ------ HELLO - контрольная общая тема, выдача задания --
             * 1 - без миссии
             * 2 - Выдача миссии подай-принеси
             * 3 - Выдача миссии побазарь-перетри
             * 4 - Выдача миссии убей-замочи */
            ( "HELLO", 1, true, "NPC_NAME! Как твои дела?" ),
            ( "HELLO", 1, false, "Я NPC_PROFESSION, занятие мне всегда надётся." ),
            ( "HELLO", 1, true, "Хорошо." ),
            ( "HELLO", 1, false, "Итак" ),

            ( "HELLO", 2, true, "NPC_NAME! Как твои дела?" ),
            ( "HELLO", 2, false, "До жути хочу MISSION_ITEM" ),
            ( "HELLO", 2, true, "Я достану тебе, что ты хочешь.Где ты последний раз видел MISSION_ITEM?|Редкая штука, забудь о ней..." ),
            ( "HELLO", 2, false, "Там|Да, пожалуй..." ),
            ( "HELLO", 2, true, "Я раздобуду для тебя то, что ты хочешь!MISSION_GIVE|END" ),
            ( "HELLO", 2, false, "Да укажут тебе путь Звёзды!|END" ),

            ( "HELLO", 3, true, "NPC_NAME! Как твои дела?" ),
            ( "HELLO", 3, false, "Ох, знаешь мне нужна помощь. Нужно, что бы кто то поговорил с MISSION_ITEM" ),
            ( "HELLO", 3, true, "Где ты последний раз видел MISSION_ITEM?|Найди кого нибудь другого, кто тебе поможет" ),
            ( "HELLO", 3, false, "Там|Я задумаюсь об этом" ),
            ( "HELLO", 3, true, "Я поговорю с нимMISSION_GIVE|END" ),
            ( "HELLO", 3, false, "Благодарю тебя!|END" ),

            ( "HELLO", 4, true, "NPC_NAME! Как твои дела?" ),
            ( "HELLO", 4, false, "Мне очень досаждает MISSION_ITEM! " ),
            ( "HELLO", 4, false, "Я хочу, что бы он умер" ),
            ( "HELLO", 4, true, "Я займусь этимнимMISSION_GIVE|Я не буду этим заниматься" ),
            ( "HELLO", 4, false, "Я жду результата!|Понимаю..." ),

            // ------ Техническая тема - ADIEU ---------------------------
            ( "ADIEU", 1, true, "Ну, пока! END_DIALOG" ),

            /* ------ Миссия подай-принеси ----------------------------------
             * 1 - Диалог с прочими - не в курсе дел
             * 2 - Диалог с выдавшим - ждёт
             * 3 - Диалог с принимающим - принимает сдачу миссии
             * 4 - Диалог с тем, у кого объект */
            ( "BRING", 1, true, "Что ты знаешь по поводу MISSION_ITEMа?" ),
            ( "BRING", 1, false, "Не могу тебе сейчас сказать чего либо ценного"),
            ( "BRING", 1, true, "Понял тебя."),
            ( "BRING", 1, false, "Что то ещё?"),

            ( "BRING", 2, true, "По поводу MISSION_ITEMа..."),
            ( "BRING", 2, false, "Что?"),
            ( "BRING", 2, true, "Я как раз собирался идти MISSION_ITEM"),
            ( "BRING", 2, false, "Я на тебя расчитываю!"),

            ( "BRING", 3, true, "По поводу MISSION_ITEMа..." ),
            ( "BRING", 3, false, "Что?"),
            ( "BRING", 3, true, "MISSION_ITEM для тебя!|Пока ничего."),
            ( "BRING", 3, false, "Благодарю тебя! Теперь у меня есть всё, что мне нужно!MISSION_COMPLETE|Я на тебя расчитываю!"),

            ( "BRING", 4, true, "Что ты знаешь по поводу MISSION_ITEMа?" ),
            ( "BRING", 4, false, "Он у меня, но просто так я тебе его не отдам. MISSION_PRICE"),
            ( "BRING", 4, true, "Конечно MISSION_PAY_PRICE!|Возможно, позже"),
            ( "BRING", 4, false, "По рукам! MISSION_COMPLETE|Тогда нам не о чем разговаривать!"),

            /* ------ Миссия побазарь - перетри -----------------------------
             * 1 - Диалог с прочими - не в курсе дел
             * 2 - Диалог с выдавшим - ждёт
             * 3 - Диалог с выдавшим - принимает сдачу миссии
             * 4 - Диалог с кем надо поговорить */
            ( "SPEAK", 1, true, "Надо поговорить о MISSION_ITEMе" ),
            ( "SPEAK", 1, false, "Не могу тебе сейчас сказать чего либо ценное"),

            ( "SPEAK", 2, true, "По поводу MISSION_ITEMа..."),
            ( "SPEAK", 2, false, "Что?"),
            ( "SPEAK", 2, true, "Я как раз собирался с ним поговорить"),
            ( "SPEAK", 2, false, "Я на тебя расчитываю!"),

            ( "SPEAK", 3, true, "По поводу MISSION_ITEMа..." ),
            ( "SPEAK", 3, false, "Что?"),
            ( "SPEAK", 3, true, "Я знаю всё, что надо!|Да ничего пока."),
            ( "SPEAK", 3, false, "Благодарю тебя! MISSION_COMPLETE|Я на тебя расчитываю!"),

            ( "SPEAK", 4, true, "MISSION_ITEM, Надо поговорить" ),
            ( "SPEAK", 4, false, "Кое что знаю. MISSION_INFORMATION"),
            ( "SPEAK", 4, true, "Спасибо!MISSION_TALKMISSION_COMPLETE"),
            ( "SPEAK", 4, false, "Что то ещё?"),

            
            /* ------ Миссия убей - замочи ----------------------------------
             * 1 - Диалог с прочими - не в курсе дел
             * 2 - Диалог с выдавшим - ждёт
             * 3 - Диалог с выдавшим - принимает сдачу миссии
             * 4 - Диалог со смертником*/
            ( "KILL", 1, true, "Ты знаешь где MISSION_ITEM?" ),
            ( "KILL", 1, false, "Нет, не в курсе"),
            ( "KILL", 1, true, "Уверен?"),
            ( "KILL", 1, false, "Абсолютно точно"),

            ( "KILL", 2, true, "По поводу MISSION_ITEMа..."),
            ( "KILL", 2, false, "Что?"),
            ( "KILL", 2, true, "Я как раз собирался найти его"),
            ( "KILL", 2, false, "Я на тебя расчитываю!"),

            ( "KILL", 3, true, "По поводу MISSION_ITEMа..." ),
            ( "KILL", 3, false, "Что?"),
            ( "KILL", 3, true, "Он убит!|Да ничего пока."),
            ( "KILL", 3, false, "Благодарю тебя! MISSION_COMPLETE|Я на тебя расчитываю!"),

            ( "KILL", 4, true, "Кое кто хочет твоей смерти" ),
            ( "KILL", 4, false, "И зачем ты мне это говоришь?"),
            ( "KILL", 4, true, "Я здесь от его имени, и теперь ты умрёшь|Просто, что бы ты был в курсе"),
            ( "KILL", 4, false, "Это мы ещё посмотрим|Благодарю. Что то ещё?"),

        };
    }

    class Conversation_Machine
    {
        readonly Condition_Checker Check = new Condition_Checker();
        readonly Stuff_Juggler Mov = new Stuff_Juggler();
        delegate void Action(); // Запись действий при выборе какой то строки
        public List<Thema> Dialog_Theme_List = new List<Thema>(0); // Общий список тем ГГ
        readonly NPC Hero;

        public Conversation_Machine(NPC Main_Hero)
        {
            Hero = Main_Hero;
            Add_Theme("HELLO");
            Add_Theme("ADIEU");
        }

        public bool Add_Theme(Mission_Engine.Acts My_Action)
        {
            Dialog_Theme_List.Add(new Thema(My_Action.Type) { bout_mission_num = My_Action.ID }) ;
            if (Dialog_Theme_List[Dialog_Theme_List.Count - 1].Count > 0) return true;
            else
            {
                Dialog_Theme_List.RemoveAt(Dialog_Theme_List.Count - 1);
                return false;
            }
        }
        public bool Add_Theme(string New_Theme)
        {
            Dialog_Theme_List.Add(new Thema(New_Theme) { bout_mission_num = -1 });
            if (Dialog_Theme_List[Dialog_Theme_List.Count - 1].Count > 0) return true;
            else
            {
                Dialog_Theme_List.RemoveAt(Dialog_Theme_List.Count - 1);
                return false;
            }
        }
        public bool Remove_Theme(int Mission_Complete_ID)
        {
            for (int i = 0; i < Dialog_Theme_List.Count; i++)
            {
                if (Dialog_Theme_List[i].bout_mission_num == Mission_Complete_ID & Dialog_Theme_List[i].Title != "HELLO" & Dialog_Theme_List[i].Title != "ADIEU")
                { Dialog_Theme_List.RemoveAt(i); return true; }
            }
            return false;
        }

        // ---------------- Dialog start with someone --------------
        public void Speak(NPC Humanoid, Mission_Engine Mission_Background)
        {
            if (Humanoid.HP == 0)
            {
                WriteLine("Мёртвые молчат!");
                return;
            }
            // ------------------------------------ Variables -------------------------------------
            int Step; //coursor in chosen theme
            string NPC_Phrase = "Здравствуй, меня зовут NPC_NAME. "; // NPC text
            List<string> Dialog_Case_Out = new List<string>(0);//strings for output 
            Action Output_Action = null;// Действия при выборе из списка
            List<Action> Dialog_Case_Action = new List<Action>(0);// действия для выбранного пункта диалога
            bool Dialog_Ended = false; // When must go out dialog

            // Метод ------------ Заполняем ключи в строке ----------------------------------------
            string Apply_Phrase(string pr_Phrase, int Connect_Mission)
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
                            case "ATACK": break;
                            case "END_DIALOG":
                                Output_Action += delegate { Dialog_Ended = true; };
                                break;
                            case "MISSION_COMPLETE":
                                if (Mission_Background[Connect_Mission].Who_Takes.ID == Humanoid.ID)
                                    Output_Action += delegate
                                {
                                    if (Mission_Background[Connect_Mission].Type == "BRING")
                                        Mov.Transfer(Hero, Humanoid, Hero.ItemIndex(Mission_Background[Connect_Mission].Item_Wanted));
                                    Mission_Background[Connect_Mission].Done = true;
                                    Mission_Background[Connect_Mission].Rewarded = true;
                                    Remove_Theme(Connect_Mission);
                                };
                                break;
                            case "MISSION_ITEM":
                                if (Mission_Background[Connect_Mission].Type == "BRING")
                                    end_string = end_string.Insert(found_on, Mission_Background[Connect_Mission].Item_Wanted.name);
                                if (Mission_Background[Connect_Mission].Type == "SPEAK")
                                    end_string = end_string.Insert(found_on, Mission_Background[Connect_Mission].Whos_Target.Name);

                                break;
                            case "MISSION_GIVE":
                                Output_Action += delegate { Mission_Background[Connect_Mission].Given = true;
                                    Add_Theme(Mission_Background[Connect_Mission]); };
                                break;
                            case "MISSION_NAME": break;
                            case "MISSION_TALK":
                                Output_Action += delegate {
                                    if(Mission_Background[Connect_Mission].Type == "SPEAK")
                                        Mission_Background[Connect_Mission].Done = true;
                                };
                                break; 
                            case "NPC_PROFESSION":
                                end_string = end_string.Insert(found_on, Humanoid.profession);
                                break;
                            case "NPC_NAME":
                                end_string = end_string.Insert(found_on, Humanoid.Name);
                                break;
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
                int choose;

                int D_Case_Num = 0; // --- Развилка в беседе --------
                List<string> Dialog_Case_Out = new List<string>(0);//strings for output 
                List<Action> Dialog_Case_Action = new List<Action>(0);// действия для выбранного пункта диалога

                while (Talk_About != "END")
                {
                    //Clean output
                    NPC_Phrase = "";
                    Dialog_Case_Out.Clear();
                    Dialog_Case_Action.Clear();

                    Talk_About = Theme_Step(Theme);
                    //---- Текст НПС --------
                    string[] D_NPC_Text = NPC_Phrase.Split(new char[] { '|' });
                    if (D_NPC_Text.Length == 1)
                        NPC_Phrase = D_NPC_Text[0];
                    else
                        NPC_Phrase = D_NPC_Text[D_Case_Num - 1];

                    if (Talk_About == "END")
                    {
                        Apply_Phrase(NPC_Phrase, Theme.bout_mission_num);
                        Output_Action?.Invoke();
                        break;
                    }

                    // --- Развилка в беседе ------- и вывод диалога -----
                    string[] D_Case_Text = Talk_About.Split(new char[] { '|' });

                    if (D_Case_Num == 0)
                        for (int i = 0; i < D_Case_Text.Length; i++)
                        {
                            Dialog_Case_Out.Add(Apply_Phrase(D_Case_Text[i], Theme.bout_mission_num));
                            Dialog_Case_Action.Add(Output_Action);
                        }
                    else
                    {
                        if (D_Case_Text[D_Case_Num - 1] == "END") break;  
                        Dialog_Case_Out.Add(Apply_Phrase(D_Case_Text[D_Case_Num - 1], Theme.bout_mission_num));
                        Dialog_Case_Action.Add(Output_Action);
                    }


                    Shog_Dialog_Window(Apply_Phrase(NPC_Phrase, Theme.bout_mission_num), Dialog_Case_Out);

                    choose = 0;
                    while (choose == 0)
                    {
                        string Enter = ReadLine();
                        if (!Int32.TryParse(Enter, out choose)) WriteLine("Not valid nember of string dialog");
                        else if (choose < 1 | choose >= Dialog_Case_Out.Count + 1)
                        { choose = 0; WriteLine("Not valid nember of string dialog"); }
                    };

                    Dialog_Case_Action[choose - 1]?.Invoke();

                    if (D_Case_Num == 0 & D_Case_Text.Length > 1)
                        D_Case_Num = choose;
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
            static void Shog_Dialog_Window(string His_Words, List<string> My_words)
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
                ResetColor(); WriteLine("");
            }

            // Метод ----------- Выбирает вариант диалога в зависимости от обстоятельств ----------
            int Dailog_Area(Thema Look_In)
            {
                Look_In.Area = 1;
                switch (Look_In.Title)
                {
                    case "ADIEU": return -1;

                    case "HELLO":
                        for (int i = 0; i < Mission_Background.Count; i++)
                        {
                            if (!Mission_Background[i].Rewarded & Mission_Background[i].Available & (Mission_Background[i].Who_Gives.ID == Humanoid.ID))
                            {
                                if (Mission_Background[i].Given) return i;

                                switch (Mission_Background[i].Type)
                                {
                                    case "BRING": Look_In.Area = 2; Look_In.bout_mission_num = i; return i;
                                    case "SPEAK": Look_In.Area = 3; Look_In.bout_mission_num = i; return i;
                                    case "KILL": Look_In.Area = 4; Look_In.bout_mission_num = i; return i;
                                }
                            }
                        }
                        return Look_In.bout_mission_num;
                    
                    default:
                        {
                            if (Mission_Background[Look_In.bout_mission_num].Whos_Target.ID == Humanoid.ID)
                            {
                                Look_In.Area = 4; return Look_In.bout_mission_num;
                            }
                            if (Mission_Background[Look_In.bout_mission_num].Who_Takes.ID == Humanoid.ID)
                            {
                                bool condition = Mission_Background[Look_In.bout_mission_num].Type switch
                                {
                                    "BRING" => Check.Item_Achieved(Mission_Background[Look_In.bout_mission_num].Item_Wanted, Hero),
                                    "SPEAK" => Mission_Background[Look_In.bout_mission_num].Done,
                                    "KILL" => Mission_Background[Look_In.bout_mission_num].Whos_Target.HP == 0,
                                };

                                if (condition)
                                {
                                    Look_In.Area = 3;
                                    return Look_In.bout_mission_num;
                                }
                                else
                                {
                                    Look_In.Area = 2;
                                    return Look_In.bout_mission_num;
                                }
                            }

        
                            return Look_In.bout_mission_num;
                        }
                }
                
            }
                


            // ---------Код диалога --------------
            while (!Dialog_Ended)
            {
                // -------------------------- Начало дивлога, построение первонаяального списка  -------------------------
                Dialog_Case_Out.Clear(); Dialog_Case_Action.Clear();

                for (int i = 0; i < Dialog_Theme_List.Count; i++)//Перебор по одной фразе
                {
                    Step = -1;
                    Dailog_Area(Dialog_Theme_List[i]);
                    Dialog_Case_Out.Add(Apply_Phrase(Theme_Step(Dialog_Theme_List[i]), Dialog_Theme_List[i].bout_mission_num));
                    Dialog_Case_Action.Add(Output_Action);
                }
                Shog_Dialog_Window(Apply_Phrase(NPC_Phrase, Dialog_Theme_List[0].bout_mission_num), Dialog_Case_Out);

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

                Dialog_Case_Action[choose - 1]?.Invoke(); // Запуск действия при выборе, если есть
                Talk_About(Dialog_Theme_List[choose - 1]); //в вариантах с 1 а в массиве с 0
                
            }
            // ------ Из диалога вышли, оплучен сигнал о выходе
            WriteLine("Bye");


        }
    }
  
    class Mission_Engine
    {
        // --------- Класс актов ------------
        public class Acts
        {
            //PARAMETRS OF SINGLE ACT
            public string Title;    // название миссии
            public string Description; // описание, что нужно сделать
            public string Type;

            public bool Available = true;  // доступность миссии для получения
            public bool Given = false;      // выдана ли миссия
            public bool Done = false;       // сделано ли условие миссии
            public bool Rewarded = false;   // получена ли награда (миссия закрыта)

            readonly public NPC Who_Gives;
            readonly public Creature Whos_Target;
            readonly public NPC Who_Takes;
            readonly public Phys_Object Item_Wanted;

            // ----- for ID ---------
            static int count_acts = 0;
            public int ID;
            // Конструктор миссии принеси-подай
            public Acts(Phys_Object Find, Creature Finder)
            {
                Description = (Finder.LifeForm == "Человек") ? Finder.Name : Finder.LifeForm;
                Description += " должен иметь: " + Find.name + " стоимостью " + Find.price;
                Title = Find.name + " нужен здесь";
                Type = "BRING";

                Who_Gives = Finder as NPC;
                Whos_Target = Finder;
                Who_Takes = Finder as NPC;
                ID = count_acts++;
                Item_Wanted = Find;
            }

            public Acts(NPC M_Give, NPC M_Talk, NPC M_Take)
            {
                Description = M_Talk.Name + "обладает нужной информацией, поговорите с ним";
                Title = M_Talk.Name +" что то знает" ;
                Type = "SPEAK";

                Who_Gives = M_Give;
                Whos_Target = M_Talk;
                Who_Takes = M_Take;
                ID = count_acts++;
                
                Item_Wanted = null;
            }
            // Конструктор миссии об убийстве
            public Acts(Creature Client, Creature Condemned)
            {
                Description = (Condemned.LifeForm == "Человек") ? Condemned.Name : Condemned.LifeForm;
                Description += " должен evthtnm";
                Title = (Condemned.LifeForm == "Человек") ? Condemned.Name : Condemned.LifeForm + " должен умереть";
                Type = "KILL";

                Who_Gives = Client as NPC;
                Whos_Target = Condemned;
                Who_Takes = Client as NPC;
                ID = count_acts++;
                Item_Wanted = null;
            }

        }
        
        // -------- Список актов -------------
        
        public List<Acts> Mission_Acts = new List<Acts>(0);
        public Acts this[int i] // модификатор доступа объект как массив [], с учётом рабочего участка
        {
            //set { Phrase_inter[ AreaStartPoint[Area - 1] + i ] = value; }
            get { return Mission_Acts[i]; }
        }
        public int Count
        { get { return Mission_Acts.Count; } }
        // -------------------- Конструкторы системы миссий -----------------
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
                //Cr[hum].Mission_Connection.Add(i);
            }
            for (int i = count_brings; i < count_brings + count_kills; i++) //добавляем миссии убей-замочи (не сделано!)
            {
                //hum = rnd.Next(Cr.Count);
                //Mission_Acts.Add(new Acts(Cr[hum]));
            }

        }

        // -------------------- Методы миссий -------------------------------
        public void Add_Bringer_Mission(Phys_Object Item, Creature Cr)
        {
            Mission_Acts.Add(new Acts(Item, Cr));
            //Cr.Mission_Connection.Add(Mission_Acts.Count -1);
        }

        public void Add_Murderr_Mission(Creature Client, Creature Condemned)
        {
            Mission_Acts.Add(new Acts(Client, Condemned));
            //Cr.Mission_Connection.Add(Mission_Acts.Count -1);
        }
        public void Add_Speaker_Mission(Creature M_Give, Creature M_Talk, Creature M_Take)
        {
            Mission_Acts.Add(new Acts(M_Give as NPC, M_Talk as NPC, M_Take as NPC));
            /*M_Give.Mission_Connection.Add(Mission_Acts.Count - 1);
            M_Talk.Mission_Connection.Add(Mission_Acts.Count - 1);
            M_Take.Mission_Connection.Add(Mission_Acts.Count - 1);*/
        }
        public void All_Missions()
        {
            WriteLine("All missions");
            for (int i = 0; i < Mission_Acts.Count; i++)
                WriteLine($"{i}) {Mission_Acts[i].Description}. Given: {Mission_Acts[i].Given}, Done: {Mission_Acts[i].Done}, Rewarded: {Mission_Acts[i].Rewarded}, ID: {Mission_Acts[i].ID}");
        }
    }


    //--------------------------------------------------------------------------------------
    //------------====-------- Program --------====-----------------------------------------
    //--------------------------------------------------------------------------------------
    class Program
    {
        static void Main()
        {
            // -----------------------------
            // ---- Initialize heroes
            // -----------------------------
            List<Creature> Peoples = new List<Creature> { new NPC("ГГ", 150), new NPC("Алгор воитель", 500), new NPC() };
            Peoples.AddRange(new Creature[] { new NPC(), new Creature(), new Creature(), new NPC() }) ;

            // -----------------------------
            // ---- Initialize objects
            // -----------------------------
            Stuff_Container Chest = new Stuff_Container("Сундучара");
            Chest.Items.AddRange(new List<Phys_Object>
            {new Phys_Object(), new Phys_Object(), new Phys_Object("Секира") { price = 10 } , new Phys_Object("Амулет", 100) });

            // -----------------------------
            // - Создание рабочих экзе-ров
            // -----------------------------
            Display_Parametrs       Display = new Display_Parametrs();          //Экзе-р для вывода списков
            Stuff_Juggler           Stuff = new Stuff_Juggler();                //Экзе-р для перемещений предметов
            Conversation_Machine    Conversation = new Conversation_Machine(Peoples[0] as NPC);  //Экзе-р для диалогов

            Display.Creatures_List(Peoples);
            // -----------------------------
            // ---- Initialize missions
            // -----------------------------
            Mission_Engine My_Story = new Mission_Engine();// (Chest, 1, Peoples, 0); // Экзе-р с миссиями по имеющимся людям и вещам

            My_Story.Add_Speaker_Mission(Peoples[1], Peoples[2], Peoples[2]);
            //My_Story.Add_Bringer_Mission(Chest.Items[2], Peoples[1]);
            //My_Story.Add_Murderr_Mission (Peoples[1], Peoples[2]);

            My_Story.All_Missions();

            Display.Stuff(Peoples[0]);
            Display.Stuff(Peoples[1]);

            Conversation.Speak(Peoples[1] as NPC, My_Story);
            My_Story.All_Missions();
            Conversation.Speak(Peoples[2] as NPC, My_Story);
            My_Story.All_Missions();

            Peoples[2].HP = 0;
            Stuff.Transfer(Chest, Peoples[0], 0);
            Stuff.Transfer(Chest, Peoples[0], 0);
            Stuff.Transfer(Chest, Peoples[0], 0);
            Stuff.Transfer(Chest, Peoples[0], 0);
            Display.Stuff(Peoples[0]);
            Display.Stuff(Peoples[1]);

            My_Story.All_Missions();
            Conversation.Speak(Peoples[2] as NPC, My_Story);
            My_Story.All_Missions();
            Conversation.Speak(Peoples[1] as NPC, My_Story);
            Display.Stuff(Peoples[0]);
            Display.Stuff(Peoples[1]);
            My_Story.All_Missions();
        }
    }
}
