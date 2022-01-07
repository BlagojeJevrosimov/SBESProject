using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ISyslogServer
    {
        [OperationContract]
        void Subscribe();
        [OperationContract]
        Event Read(int key);  // traziti po kljucu, vraca objekat
        [OperationContract]
        bool Update(Event ev);  // prosledjujemo kljuc i izmenjen objekat
        [OperationContract]
        bool Delete(int key);  // prosledjujemo kljuc
        [OperationContract]
        void ManagePermission(bool isAdd, string rolename, params string[] permissions);
        [OperationContract]
        void ManageRoles(bool isAdd, string rolename);
    }
}
