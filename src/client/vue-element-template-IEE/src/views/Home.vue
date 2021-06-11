<template>
  <el-container class="home-container">
    <!-- 侧边栏 -->
    <el-aside :width="isCollapse ? '58px' : '210px'" style="overflow:hidden;">
      <div style="display: flex; align-items: center;">
        <img :src="img" alt="" style="width:200px; height: 50px;background-color: white;padding: 5px;">
      </div>
      <!-- 侧边栏菜单区域 -->
      <el-menu background-color="#333744" text-color="#fff" active-text-color="#409EFF"
      unique-opened :collapse="isCollapse" :collapse-transition="false" :router="true" :default-active="activePath">
        <!-- 一级菜单 -->
        <el-submenu index="">
          <!-- 一级菜单的模板区域 -->
          <template slot="title">
            <!-- 图标 -->
            <i class="el-icon-s-order"></i>
            <!-- 文本 -->
            <span>请购单</span>
          </template>
          <!-- 二级菜单 -->
          <el-menu-item index="/applydetail" @click="saveNavState('/applydetail')">
            <template slot="title">
              <!-- 图标 -->
              <i class="el-icon-menu"></i>
              <!-- 文本 -->
              <span>请购单明细</span>
            </template>
          </el-menu-item>
        </el-submenu>
      </el-menu>
    </el-aside>
    <!-- 页面主体区域 -->
    <el-container>
      <!-- 头部区域 -->
      <el-header>
        <div>
          <!-- 菜单展开折叠按钮图标 -->
          <i :class="ico" style="padding: 5px; font-size: 25px; cursor: pointer;" @click="toggleCollapse"></i>
          <span>ERP辅助平台</span>
        </div>
        <el-dropdown class="avatar-container right-menu-item hover-effect" trigger="click">
          <div class="avatar-wrapper">
            <img src="../assets/user.png" class="user-avatar">
            <i style="color: #eee" class="el-icon-caret-bottom" />
          </div>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item>{{this.username}}</el-dropdown-item>
            <router-link to="/about">
              <el-dropdown-item>关于</el-dropdown-item>
            </router-link>
            <el-dropdown-item divided @click.native="logout">
              <span style="display:block;">注销</span>
            </el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </el-header>
      <!-- 右侧内容主体 -->
      <el-main>
        <!-- 路由占位符 -->
        <router-view></router-view>
      </el-main>
    </el-container>
  </el-container>
</template>

<script>
export default {
  data () {
    return {
      // 左侧菜单数据
      menulist: [],
      iconsObj: {
        125: 'iconfont icon-user',
        103: 'iconfont icon-tijikongjian',
        101: 'iconfont icon-shangpin',
        102: 'iconfont icon-danju',
        145: 'iconfont icon-baobiao'
      },
      // 是否折叠
      isCollapse: false,
      // 被激活的链接地址
      activePath: '',
      // 厂区编码
      plant: '',
      // 用户名称
      username: '',
      // log路径
      img: '',
      // 菜单展开折叠按钮图标
      ico: ''
    }
  },
  created () {
    // this.getMenuList()
    this.activePath = window.sessionStorage.getItem('activePath')
    this.plant = window.sessionStorage.getItem('plant')
    this.username = window.sessionStorage.getItem('username')
    if (this.plant === 'XUS') {
      this.img = require('../assets/XUSTitle.png')
    } else {
      this.img = require('../assets/IIETitle.png')
    }
    this.ico = 'el-icon-s-fold'
  },
  methods: {
    logout () {
      window.sessionStorage.clear()
      this.$router.push('/login')
    },
    // 获取所有的菜单
    async getMenuList () {
      const { data: res } = await this.$http.get('menus')
      if (res.meta.status !== 200) return this.$message.error(res.meta.msg)
      this.menulist = res.data
      console.log(res)
    },
    // 点击按钮，切换菜单的折叠与展开
    toggleCollapse () {
      this.isCollapse = !this.isCollapse
      if (this.isCollapse) {
        this.ico = 'el-icon-s-unfold'
      } else {
        this.ico = 'el-icon-s-fold'
      }
    },
    // 保存链接的激活状态
    saveNavState (activePath) {
      window.sessionStorage.setItem('activePath', activePath)
      this.activePath = activePath
    }
  }
}
</script>

<style lang="less" scoped>
.home-container {
  height: 100%;
}

.el-header {
  background-color: #373d41;
  display: flex;
  justify-content: space-between;
  padding-left: 0;
  align-items: center;
  color: #fff;
  font-size: 20px;
  > div {
    display: flex;
    align-items: center;
    span {
      margin-left: 15px;
    }
  }
}

.el-aside {
  background-color: #333744;
  .el-menu {
    border-right: none;
  }
}

.el-main {
  background-color: #eaedf1;
}

.iconfont {
  margin-right: 10px;
}

.avatar-container {
  margin-right: 30px;

  .avatar-wrapper {
    margin-top: 5px;
    position: relative;

    .user-avatar {
      cursor: pointer;
      width: 40px;
      height: 40px;
      border-radius: 10px;
    }

    .el-icon-caret-bottom {
      cursor: pointer;
      position: absolute;
      right: -20px;
      top: 25px;
      font-size: 12px;
    }
  }
}
</style>
