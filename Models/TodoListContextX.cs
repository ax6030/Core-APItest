using System;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Todo.Dto;

namespace Todo.Models;

public partial class TodoListContext : DbContext
{
    //大神寫出的方法，可以直接寫出方法使用
    public List<T> ExecSQL<T>(string query)
    {
        using(var command = Database.GetDbConnection().CreateCommand()) 
        { 
            command.CommandText = query;
            command.CommandType = System.Data.CommandType.Text;
            Database.OpenConnection();

            List<T> list = new List<T>();
            using(var result = command.ExecuteReader())
            {
                T obj = default(T);
                while (result.Read())
                {
                    obj = Activator.CreateInstance<T>();
                    foreach(PropertyInfo prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            if (!object.Equals(result[prop.Name], DBNull.Value))
                            {
                                prop.SetValue(obj, result[prop.Name], null);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    list.Add(obj);
                }
            }
            Database.CloseConnection();
            return list;
        }
    }
}
