using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	
	public class Consumer
	{
		string username = string.Empty;
		string id = string.Empty;

		public Consumer(string username, string id)
		{
			this.username = username;
			this.id = id;
		}
		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		public string Id
		{
			get { return id; }
			set { Id = value; }
		}

        public override string ToString()
        {
            return string.Format("Username: {0}, id: {1}", Username, Id); ;
        }
    }
}
