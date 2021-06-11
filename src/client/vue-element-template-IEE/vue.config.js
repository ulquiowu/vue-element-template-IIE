module.exports = {
	// 配置代理
	// devServer: {
	// 	proxy: 'http://192.168.0.62:8070/api'
	// },
	configureWebpack: {
		devServer: {
			proxy: {
				'/api': {
					target: 'http://192.168.0.62:8070/api', //设置你调用的接口域名和端口号 
					changeOrigin: true, //这里设置是否跨域
					pathRewrite: { // 路径重写
					'^/api': ''
					}
				}
			}
		}
	},
	// 配置自定义打包内容
	chainWebpack: config => {
		// 发布模式
		config.when(process.env.NODE_ENV === 'production', config => {
		  // html追加一个isProd属性 用于index.html来判断当前为何种模式而改变title的内容
		  config.plugin('html').tap(args => {
			args[0].isProd = true
			return args
		  })
		})
	
		// 开发模式
		config.when(process.env.NODE_ENV === 'development', config => {
		  // html追加一个isProd属性 用于index.html来判断当前为何种模式而改变title的内容
		  config.plugin('html').tap(args => {
			args[0].isProd = false
			return args
		  })
		})
	}
}