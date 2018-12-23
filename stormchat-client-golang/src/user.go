package main

//用户信息
type UserInfo struct {
	User     string //用户名
	NickName string //昵称
	Motto    string //签名
	UGroup   string //用户组。ugroup_*常量值
}

//用户组
const (
	ugroup_user  string = "User"  //普通用户
	ugroup_vip   string = "Vip"   //会员
	ugroup_admin string = "Admin" //管理员
	ugroup_group string = "Group" //群聊
)
