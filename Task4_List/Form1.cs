using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Task4_List
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_Evaluate_Click(object sender, EventArgs e)
        {
            richTextBox_output.Text = string.Empty;

            var trackableList = new TrackableList<int>();
            trackableList.ChangeEvent += TrackableList_ChangeEvent;
            trackableList.Add(10);
            trackableList.Add(20);
            trackableList.Insert(0, 13);
            trackableList[0] = 125;
            trackableList[1] = 45;
            trackableList[2] = 55;
            trackableList.Add(40);
            trackableList.Add(50);
            trackableList.Add(60);
            trackableList.Remove(50);
            trackableList.Remove(60);
            trackableList.RemoveAt(2);

            //выведем всю оставшуюся коллекцию, очистим её и выведем опять
            richTextBox_output.Text += Environment.NewLine + "=======================" + Environment.NewLine;
            for (var i = 0; i < trackableList.Count; i++)
                richTextBox_output.Text += "Index: " + i + " Value: " + trackableList[i] + Environment.NewLine;
            richTextBox_output.Text += Environment.NewLine + "=======================" + Environment.NewLine;
            trackableList.Clear();
            for (var i = 0; i < trackableList.Count; i++)
                richTextBox_output.Text += "Index: " + i + " Value: " + trackableList[i] + Environment.NewLine;
        }

        private void TrackableList_ChangeEvent<T>(object sndr, TrackableListEventArgs<T> ev)
        {
            richTextBox_output.Text += "Old Value: " + ev.OldValue + " New Value: " + ev.NewValue +
                                       " at index: " + ev.Index + " Happened: " + ev.WhatHappened + Environment.NewLine;
        }
    }

    //:byte можно и без байта, а напрямую, но мне показалось, что при передаче в эвент числа и 
    //преобразование в string - поинтереснее и код короче немного при передаче в Args
    //EvtHappened.Add = 1, EvtHappened.NewValue = 5 
    public enum EvtHappened : byte 
    {
        Nothing,
        Add,
        Clear,
        Insert,
        RemoveAt,
        NewValue,
        Remove
    };


    public class TrackableList<T> : IList<T>
    {
        public event EventHandler<TrackableListEventArgs<T>> ChangeEvent = delegate { };

        //конструктор? ссылка на объект?
        public IList<T> _list = new List<T>();

       
        //сначала не обявлял тут, а к каждому методу был конкретный var args = new.. но решил, что так лучше
        //почему индекс -1 - просто подумал, что в 0 лучше не залезать с дефолтными значениями, хотя отрабатывает и с 0
        public TrackableListEventArgs<T> Args = new TrackableListEventArgs<T>(-1,default(T),default(T), 0); //Nothing

        public void Add(T value)
        {
            _list.Add(value);
            var index = _list.Count - 1;
            var oldValue = default(T);
            Args = new TrackableListEventArgs<T>(index, oldValue, value, 1);
            OnChangeEvent(Args);
        }
     
        public void Clear()
        {
            for (var i = 0; i < _list.Count; i++)
            {
                var oldValue = _list[i];
                var index = i;
                _list[i] = default;
                Args = new TrackableListEventArgs<T>(index, oldValue, default, 2); 
                OnChangeEvent(Args);
            }
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var element in _list)
            {
                array[arrayIndex] = element;
                arrayIndex++;
            }
            //тут естественным путём, если нет ячейки массива array с очередным индексом - будет исключение,
            //такая же реакция при работе с массивом, не вижу смысла добавлять обработку исключения, если переполнение "стороннего" массива или ещё что
            //но в случае чего - эту проверку можно сделать
        }

        bool ICollection<T>.Remove(T item)
        {
            //!!!!!!
            return _list.Remove(item); //можно ли так писать, не знаю.. полагаю, что если не ругается, то логика такая:
            //если есть элемент и мы его удалили = true, если нет элемента, то false
            //будет ли подписка на событие из void Remove, пока не понял
        }

        //посокращал тут на радостях..
        public int Count => _list.Count;
        public bool IsReadOnly => _list.IsReadOnly;
        public int IndexOf(T item) => _list.IndexOf(item);
        public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Insert(int index, T value)
        {
            var oldValue = _list[index];
            _list.Insert(index, value);

            Args = new TrackableListEventArgs<T>(index, oldValue, value, 3);
            OnChangeEvent(Args);
        }

        public void RemoveAt(int index)
        {
            var oldValue = _list[index];
            _list.RemoveAt(index);
            var newValue = default(T);
            try
            {
                newValue = _list[index];
            }
            //с эксепшеном ничего не делаем, просто если будет эксепшен, то _list[index] не существует и соответственно выше определили var newValue = default(T);
            catch (Exception) { }
            Args = new TrackableListEventArgs<T>(index, oldValue, newValue, 4);
            OnChangeEvent(Args);
        }

        public T this[int index]
        {
            get => _list[index];
            set
            {
                var oldValue = _list[index];
                //нужно ли проверять, пока не знаю - зависит от задачи - если значение А заменить на равное А будет нужно,
                //к примеру, если у нас будет куча одинаковых слаться по одному индексу,
                //то с условием на неравенство можно прошляпить эту "DDoS" атаку
                //if (!object.Equals(oldValue, value))
                //{
                Args = new TrackableListEventArgs<T>(index, oldValue, value, 5);
                OnChangeEvent(Args);
                //}
                _list[index] = value;
            }
        }

        public void Remove(T value)
        {
            var index = _list.IndexOf(value);
            var oldValue = _list[IndexOf(value)];
            _list.Remove(value);
            var newValue = default(T);
            try
            {
                newValue = _list[index];
            }
            catch (Exception) { }
            Args = new TrackableListEventArgs<T>(index, oldValue, newValue, 6);
            OnChangeEvent(Args);
        }

        protected virtual void OnChangeEvent(TrackableListEventArgs<T> args)
        {
            ChangeEvent?.Invoke(this, args);
        }
    }


    public class TrackableListEventArgs<T> : EventArgs
    {
        public readonly int Index;
        public readonly T NewValue;
        public readonly T OldValue;
        public readonly string WhatHappened;

        public TrackableListEventArgs(int index, T oldValue, T newValue, byte whatHappened)
        {
            Index = index;
            OldValue = oldValue;
            NewValue = newValue;
            WhatHappened = Enum.GetName(typeof(EvtHappened), whatHappened);//тут преобразование из byte в значения string
        }
    }

    //public interface IList<T>//стандартный, не надо писать

    //4. Реализовать свой класс List, например, назови его TrackableList.
    //   Он должен быть generic, реализовать интерфейс IList<T>.
    //   Все методы (Add, Remove и так далее) реализовать использую агрегацию класса List<T>.
    //   Идея в том, что у него должно быть событие, которое вызывается на любое изменение в листе:
    //   присвоение через индексатора или удаление.
    //   Всего три типа событий(говорю три типа, но имею ввиду что событие будет одно типа и называться Change), Удаление, Вставка и Изменений.
    //   В событие через аргумент я должен знать какой тип события произошел, и какое было старое значение и новое.
    //   Если старое не известно, например, при добавление, возвращать default значение generic типа.Использовать EventHandler и enum.
}