module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  pluginOptions: {
    apollo: {
      lintGQL: false
    }
  },
  pwa: {
    name: 'IdOps',
    themeColor: '#1a237e',
    msTileColor: '#FFFFFF',
    appleMobileWebAppCapable: 'yes',
    appleMobileWebAppStatusBarStyle: 'black',

    workboxPluginMode: 'InjectManifest',
    workboxOptions: {
      swSrc: 'src/sw.js',
    }
  },
  configureWebpack: {
    devtool: 'source-map'
  },
  devServer: {
    proxy: {
      "/graphql": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
        }
      },
      "/api": {
        ws: false,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
        }
      },
      "/signal": {
        ws: true,
        changeOrigin: true,
        target: process.env.API_BASE_URL,
        headers: {
          "Authorization": "dev " + process.env.DEV_USER,
        }
      }
    }
  }
}