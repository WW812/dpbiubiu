using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biubiu.model
{
    /// <summary>
    /// 模型基本类
    /// </summary>
    public class BaseModel : INotifyPropertyChanged
    {
        public string ID { get; set; }
        public long? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public int Page { get; set; }
        private int _size = Config.PageSize;
        public int Size
        {
            get { return _size; }
            set
            {
                _size = value;
                NotifyPropertyChanged("Size");
            }
        }
        // 是否删除 0 正常 1 删除
        public int Deleted { get; set; }
        private string _note = "";
        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                NotifyPropertyChanged("Note");
            }
        }

        public BaseModel()
        {
            Size = Config.PageSize;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
