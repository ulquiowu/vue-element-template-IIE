<template>
  <div class="login_container">
    <div class="login_box">
      <!-- 头像 -->
      <div class="avator_box">
        <img :src="img" />
      </div>
      <!-- 登录表单 -->
      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginFormRules"
        class="login_form"
      >
        <!-- 用户名 -->
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            prefix-icon="el-icon-user"
            placeholder="用户名"
          ></el-input>
        </el-form-item>
        <!-- 密码 -->
        <el-form-item prop="password">
          <el-input
            type="password"
            v-model="loginForm.password"
            prefix-icon="el-icon-lock"
            placeholder="密码"
          ></el-input>
        </el-form-item>
        <!-- 厂区 -->
        <el-form-item prop="plant">
          <el-select v-model="loginForm.plant" placeholder="厂区" @change="handleSelectChange">
            <el-option label="锡凡" value="XUS"></el-option>
            <el-option label="惟勤" value="IIE"></el-option>
          </el-select>
        </el-form-item>
        <!-- 按钮 -->
        <el-form-item class="btns">
          <el-button type="primary" @click="login">登录</el-button>
          <el-button type="info" @click="resetLoginForm">重置</el-button>
        </el-form-item>
      </el-form>
    </div>
  </div>
</template>

<script>
export default {
  data () {
    return {
      //   表单数据绑定对象
      loginForm: {
        username: '',
        password: '',
        plant: ''
      },
      //   表单验证规则对象
      loginFormRules: {
        username: [
          { required: true, message: '请输入用户名', trigger: 'blur' }
        ],
        password: [{ required: true, message: '请输入密码', trigger: 'blur' }],
        plant: [{ required: true, message: '请选择厂区', trigger: 'blur' }]
      },
      img: ''
    }
  },
  created () {
    this.img = require('../assets/user.png')
  },
  methods: {
    //   重置事件
    resetLoginForm () {
      this.$refs.loginFormRef.resetFields()
    },
    // 监听下拉框选择事件
    handleSelectChange (value) {
      if (value === 'XUS') {
        this.img = require('../assets/XUS.png')
      } else {
        this.img = require('../assets/IIE.png')
      }
    },
    // 登录事件
    login () {
      this.$refs.loginFormRef.validate(async valid => {
        if (!valid) return
        const { data: res } = await this.$http.get('login/' + this.loginForm.username + '/' + this.loginForm.password + '/' + this.loginForm.plant)
        if (res.meta.status !== 200) return this.$notify.error({ title: '登录', message: '登录失败' })
        this.$notify.success({ title: '登录', message: '登录成功' })
        console.log(res)
        // 将Token保存在sessionStorage
        window.sessionStorage.setItem('token', res.data[0].token)
        window.sessionStorage.setItem('plant', this.loginForm.plant)
        window.sessionStorage.setItem('username', res.data[0].username)
        // 跳转到主页面
        this.$router.push('/home')
      })
    }
  }
}
</script>

<style lang="less" scoped>
.login_container {
  background-color: #2b4b6b;
  height: 100%;
}
.login_box {
  width: 450px;
  height: 350px;
  background-color: #fff;
  border-radius: 3px;
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);

  .avator_box {
    height: 130px;
    width: 130px;
    border: 1px solid #eee;
    border-radius: 50%;
    padding: 10px;
    box-shadow: 0 0 10px #ddd;
    position: absolute;
    left: 50%;
    transform: translate(-50%, -50%);
    background-color: #fff;
    img {
      width: 100%;
      height: 100%;
      border-radius: 50%;
    }
  }

  .btns {
    display: flex;
    justify-content: flex-end;
  }

  .login_form {
    position: absolute;
    bottom: 0;
    width: 100%;
    padding: 0 20px;
    box-sizing: border-box;
  }
}
</style>
