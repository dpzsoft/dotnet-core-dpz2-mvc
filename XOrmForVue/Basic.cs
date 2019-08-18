using System;
using System.Collections.Generic;
using System.Text;

namespace dpz2.Mvc.XOrmForVue {

    /// <summary>
    /// XOrm框架组件基类
    /// </summary>
    public abstract class Basic : dpz2.Object {

        /// <summary>
        /// 数据库定义
        /// </summary>
        public dpz2.db.Database Database { get; private set; }

        /// <summary>
        /// 初始化数据表单内容
        /// </summary>
        public dpz2.db.Row Form { get; private set; }

        /// <summary>
        /// 相关ORM表格集合
        /// </summary>
        public dpz2.db.OrmTables Tables { get; private set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public Config Config { get; private set; }

        /// <summary>
        /// 配置路径
        /// </summary>
        public string XmlPath { get; private set; }

        /// <summary>
        /// 平台名称
        /// </summary>
        public string Platform { get; private set; }

        /// <summary>
        /// 表名称
        /// </summary>
        public string Table { get; private set; }

        //下载配置文件
        private void DownloadSetting(string url, string xmlCachePath, string xmlFormPath) {
            //下载配置文件至缓存
            dpz2.Net.Http.DownFile(url, xmlCachePath);

            //将配置
            //string xmlString = dpz.IO.UTF8File.ReadAllText(xmlCachePath);
            string xmlCacheText = dpz2.File.UTF8File.ReadAllText(xmlCachePath);
            string xmlFormText = dpz2.File.UTF8File.ReadAllText(xmlFormPath);
            using (dpz2.Xml.XmlRoot xmlCache = new dpz2.Xml.XmlRoot(xmlCacheText)) {
                using (dpz2.Xml.XmlRoot form = new dpz2.Xml.XmlRoot(xmlFormText)) {
                    var xmlTable = xmlCache["table"];
                    var formTable = form["table"];
                    formTable.Attr["name"] = xmlTable.Attr["name"];
                    formTable.Attr["title"] = xmlTable.Attr["title"];

                    #region [=====初始化界面定义=====]

                    var formInterfaces = formTable["interfaces"];

                    //初始化添加界面
                    if (formInterfaces.GetNodeByAttrValue("name", "add") == null) {
                        var formInterface = formInterfaces.Add("interface");
                        formInterface.Attr["name"] = "add";
                        formInterface.Attr["type"] = "add";
                        formInterface.Attr["description"] = "添加";

                        var formInterfaceLine = formInterface.Add("line");
                        formInterfaceLine.Attr["tag-name"] = "div";

                        var formInterfaceTitle = formInterface.Add("title");
                        formInterfaceTitle.Attr["tag-name"] = "i";

                        var formInterfaceInput = formInterface.Add("content");
                        formInterfaceInput.Attr["tag-name"] = "s";
                    }

                    //初始化修改界面
                    if (formInterfaces.GetNodeByAttrValue("name", "edit") == null) {
                        var formInterface = formInterfaces.Add("interface");
                        formInterface.Attr["name"] = "edit";
                        formInterface.Attr["type"] = "edit";
                        formInterface.Attr["description"] = "修改";

                        var formInterfaceLine = formInterface.Add("line");
                        formInterfaceLine.Attr["tag-name"] = "div";

                        var formInterfaceTitle = formInterface.Add("title");
                        formInterfaceTitle.Attr["tag-name"] = "i";

                        var formInterfaceInput = formInterface.Add("content");
                        formInterfaceInput.Attr["tag-name"] = "s";
                    }

                    //初始化视图界面
                    if (formInterfaces.GetNodeByAttrValue("name", "view") == null) {
                        var formInterface = formInterfaces.Add("interface");
                        formInterface.Attr["name"] = "view";
                        formInterface.Attr["type"] = "view";
                        formInterface.Attr["description"] = "视图";

                        var formInterfaceLine = formInterface.Add("line");
                        formInterfaceLine.Attr["tag-name"] = "div";

                        var formInterfaceTitle = formInterface.Add("title");
                        formInterfaceTitle.Attr["tag-name"] = "i";

                        var formInterfaceContent = formInterface.Add("content");
                        formInterfaceContent.Attr["tag-name"] = "s";
                    }

                    //初始化列表界面
                    if (formInterfaces.GetNodeByAttrValue("name", "list") == null) {
                        var formInterface = formInterfaces.Add("interface");
                        formInterface.Attr["name"] = "list";
                        formInterface.Attr["type"] = "list";
                        formInterface.Attr["description"] = "列表";

                        var formInterfaceVue = formInterface.Add("vue");
                        formInterfaceVue.Attr["for"] = "(row,index) in list";
                        formInterfaceVue.Attr["item"] = "row";
                        formInterfaceVue.Attr["key"] = "row.ID";

                        var formInterfaceVueOrder = formInterfaceVue.Add("order");
                        formInterfaceVueOrder.Attr["name"] = "orderField";
                        formInterfaceVueOrder.Attr["type"] = "orderType";
                        formInterfaceVueOrder.Attr["click"] = "onOrder";

                        var formInterfaceRow = formInterfaces.Add("row");
                        var formInterfaceCell = formInterfaces.Add("cell");
                    }

                    #endregion

                    #region [=====初始化字段定义=====]

                    var formFields = formTable["fields"];
                    foreach (var xmlField in xmlTable.Nodes) {
                        if (xmlField.Name.ToLower() == "field") {
                            string fieldName = xmlField.Attr["name"];
                            var formField = formFields.GetNodeByAttrValue("name", fieldName);
                            if (formField == null) {
                                formField = formFields.Add("field");
                                formField.Attr["name"] = fieldName;
                                formField.Add("data");
                            }

                            var formFieldAdd = formField.Nodes.GetFirstNodeByName("add");
                            if (formFieldAdd == null) {
                                formFieldAdd = formField.Add("add");
                                formFieldAdd.Attr["type"] = "input";
                                formFieldAdd.Attr["save"] = "form";
                                formFieldAdd.Attr["model"] = "form." + fieldName;
                            }

                            var formFieldEdit = formField.Nodes.GetFirstNodeByName("edit");
                            if (formFieldEdit == null) {
                                formFieldEdit = formField.Add("edit");
                                formFieldEdit.Attr["type"] = "input";
                                formFieldEdit.Attr["save"] = "form";
                                formFieldEdit.Attr["model"] = "form." + fieldName;
                            }

                            var formFieldView = formField.Nodes.GetFirstNodeByName("view");
                            if (formFieldView == null) {
                                formFieldView = formField.Add("view");
                                formFieldView.Attr["type"] = "text";
                            }

                            var formFieldList = formField.Nodes.GetFirstNodeByName("list");
                            if (formFieldList == null) {
                                formFieldList = formField.Add("list");
                                formFieldList.Attr["type"] = "text";
                                formFieldList.Attr["width"] = "100px";
                            }


                            formField.Attr["title"] = xmlField.Attr["title"];
                            var xmlFieldData = xmlField["data"];
                            var formFieldData = formField["data"];
                            formFieldData.Attr["type"] = xmlFieldData.Attr["type"];
                            formFieldData.Attr["size"] = xmlFieldData.Attr["size"];
                            formFieldData.Attr["float"] = xmlFieldData.Attr["float"];
                        }
                    }

                    #endregion

                    dpz2.File.UTF8File.WriteAllText(xmlFormPath, form.InnerXml);
                    //form.Save();
                }
            }
        }

        /// <summary>
        /// 对象实例化
        /// </summary>
        /// <param name="db"></param>
        /// <param name="form"></param>
        /// <param name="tables"></param>
        /// <param name="config"></param>
        /// <param name="plmName"></param>
        /// <param name="tabName"></param>
        public Basic(dpz2.db.Database db, dpz2.db.OrmTables tables, dpz2.db.Row form, Config config, string plmName, string tabName) {
            //controller = ctl;
            this.Database = db;
            this.Form = form;
            this.Tables = tables;
            this.Config = config;
            this.Platform = plmName;
            this.Table = tabName;
            //this.IsZeroToEmptyInTable = false;

            //下载地址
            string url = $"{config.Url}/{plmName}/{tabName}.xml";

            //配置目录设定
            string xmlCacheFolderPath = $"{config.CachePath}/{plmName}";
            string xmlFormFolderPath = $"{config.Path}/{plmName}";
            if (!System.IO.Directory.Exists(xmlCacheFolderPath)) System.IO.Directory.CreateDirectory(xmlCacheFolderPath);
            if (!System.IO.Directory.Exists(xmlFormFolderPath)) System.IO.Directory.CreateDirectory(xmlFormFolderPath);

            //配置路径设定
            string xmlCachePath = $"{xmlCacheFolderPath}/{tabName}.xml";
            string xmlFormPath = $"{xmlFormFolderPath}/{tabName}.xml";

            //配置文件初始化
            if (!System.IO.File.Exists(xmlCachePath)) {
                DownloadSetting(url, xmlCachePath, xmlFormPath);
            }

            this.XmlPath = xmlFormPath;

        }

    }
}
