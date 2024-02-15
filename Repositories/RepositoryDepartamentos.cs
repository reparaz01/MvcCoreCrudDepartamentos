using Microsoft.AspNetCore.Http.HttpResults;
using MvcCoreCrudDepartamentos.Models;
using System.Data;
using System.Data.SqlClient;

#region PROCEDIMIENTOS ALMACENADOS


/*
create procedure SP_INSERTARDEPARTAMENTO
(@NOMBRE NVARCHAR(50), @LOCALIDAD NVARCHAR(50))
AS
    DECLARE @nextId INT
	SELECT @nextId = max(DEPT_NO) +1 FROM DEPT
	INSERT INTO DEPT VALUES (@nextId, @NOMBRE, @LOCALIDAD)
GO
*/


#endregion

namespace MvcCoreCrudDepartamentos.Repositories
{
    public class RepositoryDepartamentos
    {

        SqlConnection cn;
        SqlCommand com;
        SqlDataReader reader;

        public RepositoryDepartamentos()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=HOSPITALES;Persist Security Info=True;User ID=SA;Password=MCSD2023";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
        }


        public async Task <List<Departamento>> GetDepartamentosAsync()
        {
            string sql = "select * from DEPT";
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql; 
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();
            List<Departamento> departamentos = new List<Departamento>();
            while(await this.reader.ReadAsync())
            {
                Departamento dept = new Departamento();
                dept.IdDepartamento = int.Parse(this.reader["DEPT_NO"].ToString());
                dept.Nombre = this.reader["DNOMBRE"].ToString();
                dept.Localidad = this.reader["LOC"].ToString();
                departamentos.Add(dept);    
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            return departamentos;

        }


        public async Task<Departamento> FindDepartamentoAsync(int id)
        {
            string sql = "select * from DEPT where DEPT_NO = @id";
            this.com.Parameters.AddWithValue("@id", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            this.reader = await this.com.ExecuteReaderAsync();

            Departamento dept = null;
            if(await this.reader.ReadAsync()) 
            {
                dept = new Departamento();
                dept.IdDepartamento = int.Parse(this.reader["DEPT_NO"].ToString());
                dept.Nombre = this.reader["DNOMBRE"].ToString();
                dept.Localidad = this.reader["LOC"].ToString();
            }
            else
            {
                //NO HAY DATOS
            }

            await this.reader.CloseAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
            return dept;

        }

        public async Task InsertDepartamentoAsync(string nombre, string localidad)
        {
            this.com.Parameters.AddWithValue("@NOMBRE", nombre);
            this.com.Parameters.AddWithValue("@LOCALIDAD", localidad);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_INSERTARDEPARTAMENTO";
            await this.cn.OpenAsync();  
            int af = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

        }

        public async Task UpdateDepartamentoAsync(int id, string nombre, string localidad)
        {
            string sql = "update DEPT set DNOMBRE=@NOMBRE, LOC=@LOCALIDAD where DEPT_NO =@id";

            this.com.Parameters.AddWithValue("@NOMBRE", nombre);
            this.com.Parameters.AddWithValue("@LOCALIDAD", localidad);
            this.com.Parameters.AddWithValue("@id", id);

            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;

            await this.cn.OpenAsync();
            int af = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();

        }

        public async Task DeleteDepartamentoAsync(int id)
        {
            string sql = "delete from DEPT where DEPT_NO=@id";
            this.com.Parameters.AddWithValue("@id", id);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            int af = await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

    }
}
