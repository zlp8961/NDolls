﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NDolls.Data.Attribute
{
    public class AssociationAttribute : System.Attribute
    {
        /// <summary>
        /// 关联特性构造函数
        /// </summary>
        /// <param name="refField">与关联对象关联的字段名</param>
        /// <param name="associationType">关联对象类别</param>
        /// <param name="cascadeType">关联对象级联操作方式</param>
        public AssociationAttribute(string refField, AssociationType associationType, CascadeType cascadeType)
        {
            this.RefField = refField;
            this.AssType = associationType;
            this.CasType = cascadeType;
        }

        /// <summary>
        /// 关联特性构造函数(默认只查询关联对象)
        /// </summary>
        /// <param name="fieldName">与关联对象关联的字段名</param>
        /// <param name="associationType">关联对象类别</param>
        public AssociationAttribute(string refField, AssociationType associationType)
        {
            this.RefField = refField;
            this.AssType = associationType;
            this.CasType = CascadeType.SELECT;
        }

        /// <summary>
        /// 关联特性构造函数
        /// </summary>
        /// <param name="refField">与关联对象关联的主对象字段名</param>
        /// <param name="fieldName">关联对象字段名</param>
        /// <param name="associationType">关联对象类别</param>
        /// <param name="cascadeType">关联对象级联操作方式</param>
        public AssociationAttribute(string refField, string fieldName, AssociationType associationType, CascadeType cascadeType)
        {
            this.RefField = refField;
            this.FieldName = fieldName;
            this.AssType = associationType;
            this.CasType = cascadeType;
        }

        /// <summary>
        /// 与关联对象关联的字段名
        /// </summary>
        public string RefField { get; set; }

        /// <summary>
        /// 关联对象字段名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 关联关系类别
        /// </summary>
        public AssociationType AssType { get; set; }

        /// <summary>
        /// 关联对象级联操作方式
        /// </summary>
        public CascadeType CasType { get; set; }


    }
}
