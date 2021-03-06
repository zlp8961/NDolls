﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDolls.Data.Entity;
using System.Data.SqlClient;
using NDolls.Data.Util;

namespace NDolls.Data
{
    /// <summary>
    /// 数据库事务处理
    /// </summary>
    class DBTransaction
    {
        private static readonly string insertSQL = "INSERT INTO {0}({1}) VALUES({2})";
        private static readonly string updateSQL = "UPDATE {0} SET {1} WHERE {2}";
        private static readonly string deleteSQL = "DELETE FROM {0} WHERE {1}";

        private List<SqlParameter> pars = null;//sql命令参数集合
        private StringBuilder fs = null;//字段sql
        private StringBuilder vs = null;//值sql
        private string sql = null;
        private string condition = null;
        private string tableName = null;

        private List<OptEntity> entities = new List<OptEntity>();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="entities">操作对象集合</param>
        public DBTransaction(List<OptEntity> entities)
        {
            this.entities = entities;
        }

        /// <summary>
        /// 事务执行
        /// </summary>
        /// <returns>事务执行结果</returns>
        public bool Excute()
        {
            using (SqlHelper.Connection)
            {
                SqlHelper.Connection.Open();
                SqlTransaction tran = SqlHelper.Connection.BeginTransaction();

                try
                {
                    foreach (OptEntity item in entities)
                    {
                        tableName = EntityUtil.GetTableName(item.Entity.GetType());
                        resetVars();//重置参数(新对象重新赋值)

                        switch (item.Type)
                        {
                            case OptType.Create://新增                            
                                sql = getCreateSQL(item.Entity);
                                break;
                            case OptType.Update://修改
                                sql = getUpdateSQL(item.Entity);
                                break;
                            case OptType.Save:
                                break;
                            case OptType.Delete://删除
                                sql = getDeleteSQL(item.Entity);
                                break;
                        }

                        if (!String.IsNullOrEmpty(sql))
                            SqlHelper.ExecuteNonQuery(tran, System.Data.CommandType.Text, sql, pars);
                    }

                    tran.Commit();
                    return true;
                }
                catch(Exception ex)
                {
                    tran.Rollback();
                    throw (ex);
                }
            }
        }

        #region 辅助方法

        private void resetVars()
        {
            sql = "";
            pars = new List<SqlParameter>();
            fs = new StringBuilder();//字段sql
            vs = new StringBuilder();//值sql
        }

        private string getCreateSQL(EntityBase entity)
        {
            string identitySql="";
            foreach (DataField field in EntityUtil.GetDataFields(entity))//构造SQL参数集合
            {
                if (field.FieldValue != null && !field.IsIdentity)//字段值不为空 且 不是标识
                {
                    pars.Add(new SqlParameter(field.FieldName, field.FieldValue));
                    fs.Append(field.FieldName + ",");
                    vs.Append("@" + field.FieldName + ",");
                }
                else if (field.FieldValue != null && !field.IsIdentity)//如果有标识字段，返回最新标识值
                {
                    identitySql = ";SELECT @@IDENTITY";
                }
            }

            return String.Format(insertSQL, tableName, fs.ToString().TrimEnd(','), vs.ToString().TrimEnd(',')) + identitySql ;
        }

        private string getUpdateSQL(EntityBase entity)
        {
            condition = "";

            foreach (DataField field in EntityUtil.GetDataFields(entity))//构造SQL参数集合
            {
                if (field.FieldValue != null)
                {
                    pars.Add(new SqlParameter(field.FieldName, field.FieldValue));

                    if (EntityUtil.GetPrimaryKey(tableName).Contains(field.FieldName))
                    {
                        condition += field.FieldName + "=@" + field.FieldName + " AND ";
                    }
                    else
                    {
                        if (!field.IsIdentity)
                        {
                            fs.Append(field.FieldName + "=@" + field.FieldName + ",");
                        }
                    }
                }
            }

            return String.Format(updateSQL, tableName, fs.ToString().Trim(','), condition.Substring(0, condition.LastIndexOf("AND")));//生成UPDATE语句
        }

        private string getDeleteSQL(EntityBase entity)
        {
            string pk = EntityUtil.GetPrimaryKey(tableName);
            string[] pks = pk.Split(',');
            pars = new List<SqlParameter>();
            condition = "";

            for (int i = 0; i < pks.Length; i++)
            {
                condition += pks[i] + "=@" + pks[i] + " AND ";
                pars.Add(new SqlParameter(pks[i], EntityUtil.GetValueByField(entity, pks[i])));
            }

            return String.Format(deleteSQL, tableName, condition.Substring(0, condition.LastIndexOf(',')));
        }

        #endregion

    }
}
