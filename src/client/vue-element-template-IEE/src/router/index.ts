import Vue from 'vue'
import VueRouter, { RouteConfig } from 'vue-router'
import Login from '../views/Login.vue'
import Home from '../views/Home.vue'
import About from '../views/About.vue'
import ApplyDetail from '../views/apply/ApplyDetail.vue'

Vue.use(VueRouter)

const routes: Array<RouteConfig> = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login, meta: { title: '登录' } },
  {
    path: '/home',
    component: Home,
    redirect: '/about',
    children: [
      { path: '/about', component: About, meta: { title: '关于' } },
      { path: '/applydetail', component: ApplyDetail, meta: { title: '请购单明细' } }
    ]
  }
]

const router = new VueRouter({
  routes
})

// 挂载路由导航守卫
router.beforeEach((to, from, next) => {
  let strTitle = ''
  if (process.env.NODE_ENV === 'production') {
    strTitle = 'ERP辅助平台'
  } else {
    strTitle = 'dev-ERP辅助平台'
  }
  document.title = strTitle + '-' + to.meta.title
  // to 将要跳转的访问路径
  // from 从哪个路径跳转而来
  // next 放行
  // next() 放行
  // next('/login') 强制跳转到某个页面
  if (to.path === '/login') return next()
  // 获取 token
  const tokenStr = window.sessionStorage.getItem('token')
  // 判断token是否存在，没有则跳转到登录界面
  if (!tokenStr) return next('/login')
  console.log(document)
  console.log(document.title)
  next()
})

export default router
