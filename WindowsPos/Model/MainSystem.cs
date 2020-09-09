using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPos.Model
{
    class MainSystem
    {
        private static MainSystem _instance;

        public Member _member;
        //public List<Table> _tablelist;  // 현재 생성되어있는 테이블 목록
        public DataTable _tablelist;
        //public List<Food> _menulist;
        public Dictionary<string, int> _productList = new Dictionary<string, int>();
        public MainSystem() {}

        public static MainSystem GetInstance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainSystem();
                return _instance;
            }            
        }

        public void SetMember(DataRow dtRow)
        {
            _member = new Member(dtRow);
        }
        //public void SetTableList(List<Table> tablelist)
        //{
        //    _tablelist = tablelist;
        //}
        public void SetTableList(DataTable tablelist)
        {
            _tablelist = tablelist;
        }

        public void SetMenuList(List<Food> menulist)
        {
            //_menulist = menulist;
        }
    }
}
