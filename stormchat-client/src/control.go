package main

//宏
const (
	//最大包头长度
	max_head_length uint32 = 4096 //4K
	//最大消息长度
	max_message_length uint32 = 0x6400000 //64M
	//最大头像大小
	max_photo_size uint32 = 0x400000 //4M
	//服务器地址
	server_addr string = "mattuy.top:3727"
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

//帮助
const help_message string = `
 欢迎使用StormChat!
 输入“help”查看命令列表
 输入“.”退出命令模式，退出后可输入“.”重新进入命令模式
 聊天时可在行末输入‘\’以换行输入消息
`

//指令列表
const command_list string = `
 指令列表：
 .           退出命令模式                   login     登录
 to          设定聊天对象                   logout    注销
 friends     查看用户列表                   help      显示命令列表
 scroll      消息面板翻页                   clear     清空消息记录
 exit        退出程序                      
 CTRL + C    强制退出
  
`
