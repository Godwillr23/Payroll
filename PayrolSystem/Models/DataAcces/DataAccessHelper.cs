using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrolSystem.Models.DataAcces
{
    public class DataAccessHelper
    {
        readonly ContextEntities _dbContext = new ContextEntities();

        public List<ClientDetails> FetchClients()
        {
            return _dbContext.ClientDetail.ToList();
        }

        public List<CompanyDetails> FetchDepartments()
        {
            return _dbContext.CompanyDetail.ToList();
        }

        public int AddEmployee(ClientDetails client)
        {
            _dbContext.ClientDetail.Add(client);
            _dbContext.SaveChanges();
            return client.ClientID;
        }

        public int AddDepartment(CompanyDetails company)
        {
            _dbContext.CompanyDetail.Add(company);
            _dbContext.SaveChanges();
            return company.CompanyID;
        }
    }
}