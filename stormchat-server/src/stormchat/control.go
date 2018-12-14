package main

//宏
const (
	//是否为服务模式，调试时置为false以输出调试信息
	serveMode = false
	//最大包头长度
	max_head_length uint32 = 4096 //4K
	//最大消息长度
	max_message_length uint32 = 0x6400000 //64M
	//最大头像大小
	max_photo_size uint32 = 0x400000 //4M
	//最大数据等待时间，单位秒。客户端应发送心跳包以保持连接。调试，先设为100年
	timeout_message = 100 * 365 * 24 * 60 * 60
)

//用户组
const (
	ugroup_user  string = "User"  //普通用户
	ugroup_vip   string = "Vip"   //会员
	ugroup_admin string = "Admin" //管理员
	ugroup_group string = "Group" //群聊
)

//消息头的通用字段名称
const (
	keyname_operation string = "Operation" //操作名
	keyname_token     string = "Token"
)

//控制消息
const (
	operation_panic           string = "Panic"
	operation_trans_message   string = "TransMessage"
	operation_send_message    string = "SendMessage"
	operation_ping            string = "Ping"
	operation_login           string = "Login"
	operation_logout          string = "Logout"
	operation_offline         string = "Offline"
	operation_get_user_list   string = "GetUserList"
	operation_update_userinfo string = "UpdateUserInfo"
)

//字符串常量
const (
	//日志文件。交互模式时无效（输出到os.Stdout）
	str_log_file string = "/var/log/stormchat.log"
	//mysql连接字符串
	str_db_conn_str string = "stormchat:stormchat@tcp(localhost:3306)/stormchat?charset=utf8"
	//登陆
	str_sql_login string = "SELECT `User`, `NickName`, `Motto`, `UGroup` FROM `user` WHERE `User`=? AND `Pwd`=? LIMIT 1"
	//获取用户信息
	str_sql_query_user string = "SELECT `User`, `NickName`, `Motto`, `UGroup` FROM `user` WHERE `User`=? LIMIT 1"
	//获取头像
	str_sql_get_photo string = "SELECT `Photo` FROM `user` WHERE `User`=?"
	//设置昵称
	str_sql_update_nickname string = "UPDATE `user` SET `NickName`=? WHERE `User`=?"
	//设置密码
	str_sql_update_password string = "UPDATE `user` SET `Pwd`=? WHERE `User`=?"
	//设置签名
	str_sql_update_motto string = "UPDATE `user` SET `Motto`=? WHERE `User`=?"
	//设置头像
	str_sql_update_photo string = "UPDATE `user` SET `Photo`=? WHERE `User`=?"
	//获取用户列表（有序）
	str_sql_get_users string = "SELECT `User`, `NickName`, `Motto`, `UGroup` FROM `user` ORDER BY `User`"
	//SQL语句-消息入库
	str_sql_save_message string = "INSERT INTO `message` (`When`, `From`, `To`, `Msg`) VALUES(?, ?, ?, ?)"
	//SQL语句-消息出库
	str_sql_get_message string = "SELECT `Id`, `When`, `From`, `To`, `Msg` FROM `message` WHERE `To`=? ORDER BY `When`"
	//SQL语句-消息已送出，清除
	str_sql_delete_message string = "DELETE FROM `message` WHERE `Id`=?"
)
