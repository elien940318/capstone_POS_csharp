using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPos.Model
{
    class Member
    {
        private string id;
        private string pw;
        private string name;
        private string email;
        private DateTime sign;

        
        public Member(string id, string pw)
        {
            this.id = id;
            this.pw = pw;
        }
        public Member(string id, string pw, string str) : this(id, pw)
        {
            if (str.IndexOf("@") != -1)
                email = str;
            else
                name = str;
        }        
        public Member(DataRow temp)
        {
            id = temp[0].ToString();
            pw = temp[1].ToString();
            name = temp[2].ToString();
            email = temp[3].ToString();
            sign = DateTime.Parse(temp[4].ToString());
        }
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string Password
        {
            get { return pw; }
            set { pw = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public string Email 
        {
            get { return email; }
            set { email = value; }
        }
    }
}
