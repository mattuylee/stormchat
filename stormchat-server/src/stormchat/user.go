package main

import (
	"encoding/base64"
	"unicode/utf8"
)

//用户信息
type UserInfo struct {
	User     string //用户名
	Nickname string //昵称
	Motto    string //签名
	UGroup   string //用户组。ugroup_*常量值
}

//根据用户名查询一个用户。如果用户不存在则返回nil
func QueryUserInfo(userName string) *UserInfo {
	user := new(UserInfo)
	err := server.db.QueryRow(str_sql_query_user, userName).Scan(&user.User, &user.Nickname, &user.Motto, &user.UGroup)
	if err != nil {
		return nil
	} else {
		return user
	}
}

//获取用户头像
func GetUserPhoto(userName string) []byte {
	var photo string
	err := server.db.QueryRow(str_sql_get_photo, userName).Scan(&photo)
	if err != nil {
		return nil
	}
	photoData, err := base64.StdEncoding.DecodeString(photo)
	if err != nil {
		return nil
	}
	return photoData
}

//登录，成功返回用户信息，失败返回nil
func Login(userName string, pwd string) *UserInfo {
	user := new(UserInfo)
	err := server.db.QueryRow(str_sql_login, userName, pwd).Scan(&user.User, &user.Nickname, &user.Motto, &user.UGroup)
	if err != nil {
		if err.Error() == "sql: no rows in result set" {
			Debug(err.Error())
		} else {
			WriteLog("[Login]Database Error: " + err.Error())
		}
		return nil
	} else {
		return user
	}
}

//获取用户列表
func GetUserList() []UserInfo {
	users := make([]UserInfo, 0, 10)
	rows, err := server.db.Query(str_sql_get_users)
	defer rows.Close()
	if err != nil {
		WriteLog("Database Error: " + err.Error())
		return nil
	}
	for rows.Next() {
		user := new(UserInfo)
		scanErr := rows.Scan(&user.User, &user.Nickname, &user.Motto, &user.UGroup)
		if scanErr != nil {
			WriteLog("Database Error - Scan Error:  " + scanErr.Error())
			return nil
		}
		users = append(users, *user)
	}
	err = rows.Err()
	if err != nil {
		WriteLog("Database Error: " + err.Error())
		return nil
	}
	return users
}

//更改昵称
func (user *UserInfo) UpdateNickName(newNickName string) (errText string) {
	if length := utf8.RuneCountInString(newNickName); length == 0 || length > 12 {
		return "Illeagal NickName"
	}
	_, err := server.db.Exec(str_sql_update_nickname, newNickName, user.User)
	if err != nil {
		WriteLog("[UpdateNickName]DB Error: " + err.Error())
		return "Database Error"
	}
	return ""
}

//更新密码
func (user *UserInfo) UpdatePassword(newPassword string) (errText string) {
	if length := len([]rune(newPassword)); length == 0 || length > 16 {
		return "Illeagal Password"
	}
	_, err := server.db.Exec(str_sql_update_password, newPassword, user.User)
	if err != nil {
		WriteLog("[UpdatePassword]DB Error: " + err.Error())
		return "Database Error"
	}
	return ""
}

//更新签名
func (user *UserInfo) UpdateMotto(newMotto string) (errText string) {
	if length := utf8.RuneCountInString(newMotto); length > 32 {
		return "Motto is too long"
	}
	_, err := server.db.Exec(str_sql_update_motto, newMotto, user.User)
	if err != nil {
		WriteLog("[UpdateMotto]DB Error: " + err.Error())
		return "Database Error"
	}
	return ""
}

//更新头像
func (user *UserInfo) UpdatePhoto(data []byte) (errText string) {
	if uint32(len(data)) > max_photo_size {
		return "New photo is too large"
	}
	_, err := server.db.Exec(str_sql_update_photo, base64.StdEncoding.EncodeToString(data), user.User)
	if err != nil {
		WriteLog("[UpdatePhoto]DB Error: " + err.Error())
		return "Database Error"
	}
	return ""
}
