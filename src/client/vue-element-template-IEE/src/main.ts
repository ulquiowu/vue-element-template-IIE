import Vue from 'vue'
import './plugins/axios'
import App from './App.vue'
import router from './router'
import store from './store'
import './assets/css/global.css'
import './plugins/element.js'
import axios from 'axios'
import { Notification } from 'element-ui'

// axios 请求拦截器
axios.interceptors.request.use(config => {
  // 需要判断token是否存在，否则登录方法会因为空数据引起异常
  const tokenStr = window.sessionStorage.getItem('token')
  if (tokenStr) {
    config.headers.Authorization = `${tokenStr}`
  }
  console.log(config)
  return config
})
// axios 响应拦截器
axios.interceptors.response.use(config => {
  // 如果 token 过期就清空 token 跳转到登录页
  if (config.data === '您尚未登录,请登录后重新尝试') {
    window.sessionStorage.clear()
    Notification.error({ title: '登录', message: '登录已过期,请登录后重新尝试' })
    router.push('/login')
    location.reload()
  }
  console.log(config)
  return config
})

Vue.prototype.$http = axios
axios.defaults.baseURL = '/api'

Vue.config.productionTip = false

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
