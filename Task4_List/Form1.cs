using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            richTextBox_out.Text = string.Empty;
            
            var temp = new TrackableList<int>();

            temp.Add(5);
            temp.Add(10);
            temp.Add(5);
            temp.Add(10);
            temp.Add(5);
            temp.Add(10);
            temp.Add(5);
            temp.Add(10);
            temp.RemoveAt(4);

            richTextBox_out.Text += temp.ShowAll() + Environment.NewLine;
            richTextBox_out.Text += temp.IsReadOnly;

            //richTextBox_out.Text += temp.OnChange;

        }
    }

    internal enum IndexList
    {Max = 2000, Min = 0 }

 
    //public interface IList<T>//стандартный, не надо писать


    //4. Реализовать свой класс List, например, назови его TrackableList.
    //   Он должен быть generic, реализовать интерфейс IList<T>.
    //   Все методы (Add, Remove и так далее) реализовать использую агрегацию класса List<T>.
    //   Идея в том, что у него должно быть событие, которое вызывается на любое изменение в листе:
    //   присвоение через индексатора или удаление.
    //   Всего три типа событий(говорю три типа, но имею ввиду что событие будет одно типа и называться Change), Удаление, Вставка и Изменений.
    //   В событие через аргумент я должен знать какой тип события произошел, и какое было старое значение и новое.
    //   Если старое не известно, например, при добавление, возвращать default значение generic типа.Использовать EventHandler и enum.



    //#region TrackableList
    public class TrackableList<T> : IList<T>
// where T: IList<T>
    {
        int _count;
        //private IList<T> _array = new T[(int)IndexList.Max-(int)IndexList.Min];
        //private IList<T> _array_old = new T[(int)IndexList.Max - (int)IndexList.Min];
        public IList<T> _array = new List<T>();
        public IList<T> _array_old = new List<T>();

        public TrackableList()
        {
            _count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _array.GetEnumerator(); 
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }

        public void Add(T item)
        {
           foreach (var element in _array)
            {
                _array_old = _array;
            }
            _array.Add(item);
            _count++;
        }

        public void Clear()
        {
            foreach (var element in _array)
            {
                _array_old = _array;
                _array = default;//!!!!!!!!! C#7.1+
            }
            _count = 0;
        }

        public bool Contains(T item)
        {
            return _array.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var element in _array)
            {
                array[arrayIndex] = element;
                arrayIndex++;
            }
            //тут естественным путём, если нет ячейки массива array с очередным индексом - будет исключение,
            //такая же реакция при работе с массивом, не вижу смысла добавлять обработку исключения, если переполнение или ещё что
        }

        public bool Remove(T item)
        {
            return _array.Remove(item); //можно ли так писать, не знаю.. полагаю, что если не ругается, то логика такая:
            //если есть элемент и мы его удалили = true, если нет элемента, то false
        }

        public int Count => _array.Count;

        public bool IsReadOnly => _array.IsReadOnly;

        public int IndexOf(T item)
        {
            if (_array.Contains(item))
                return _array.IndexOf(item);
            return -1;
        }

        public void Insert(int index, T item)
        {
            for (int i=index;i<_array.Count;i++) //все, начиная с индекса переписываем в old
                _array_old[i] = _array[i];
            _array.Insert(index, item);              
        }

        public void RemoveAt(int index)
        {
            if ((index >= 0) && (index < Count))
            {
                foreach (var element in _array)
                {
                    _array_old = _array;
                }
                for (int i = index; i < Count - 1; i++)
                {
                    _array[i] = _array[i + 1];
                }
                _count--;
            }
        }

        public T this[int index]
        {
            get => _array[index];
            set => _array[index] = value;
        }


        public string ShowCurrent(int index)
        { 
           return  "Old Value: " + _array_old[index] + " New Value: " + _array[index] + " Currrent Index: " + index + Environment.NewLine;
        }

        public string ShowAll()
        {
            string result = String.Empty;
            for (int i = 0; i < _array.Count; i++)
                result+= ShowCurrent(i);
            return result;
        }

        

        //Code\Chapter 09\FunWithObservableCollection
        //уже пришлось заюзать исходники из Троелсена.. всё равно что-то не получается
        //заодно стал юзать регионы - хоть какой-то плюс :)
        public string OnCh_result = String.Empty;

        //#region OnChange event handler
        public EventHandler OnChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
           
            // какое было действие
            OnCh_result = "Action for this event: " + e.Action;

            // что-то удалили
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                OnCh_result += "Here are the OLD items: ";
                foreach (T[] p in e.OldItems)
                {
                    OnCh_result += p.ToString() + " Index:" + e.NewStartingIndex + Environment.NewLine;
                }
                OnCh_result += Environment.NewLine;
            }

            // что-то добавили
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                OnCh_result += "Here are the NEW items: ";
                foreach (T[] p in e.NewItems)
                {
                    OnCh_result += p.ToString() +" Index:"+ e.NewStartingIndex + Environment.NewLine;
                }
            }
            
            //что-то заменили
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                OnCh_result += "Here are the NEW items: ";
                foreach (T[] p in e.NewItems)
                {
                    OnCh_result += p.ToString() + " Index:" + e.NewStartingIndex + Environment.NewLine;
                }
            }
            return null;
        }
        //#endregion


        
    }
    //#endregion
    
     
}
