using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusMate.Model
{
    public class Task
    {
        private int _id;
        private string _title;
        private int _categoryId;
        private string _categoryName;
        private DateTime _date;
        private bool _isDone;

        public int Id { get { return _id; } set { _id = value; } }
        public string Title { get { return _title; } set { _title = value; } }
        public int CategoryId { get { return _categoryId; } set { _categoryId = value; } }
        public string CategoryName { get { return _categoryName; } set { _categoryName = value; } }
        public DateTime Date { get { return _date; } set { _date = value; } }
        public bool IsDone { get { return _isDone; } set { _isDone = value; } }
    }
}
