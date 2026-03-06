const tokenService = {
  _accessToken: null,
  _refreshToken: null,

  get accessToken() {
    return this._accessToken
  },

  set accessToken(value) {
    this._accessToken = value
  },

  get refreshToken() {
    return this._refreshToken
  },

  set refreshToken(value) {
    this._refreshToken = value
  },

  setFromStorage() {
    const auth = localStorage.getItem('auth')
    if (auth) {
      const { accessToken, refreshToken } = JSON.parse(auth)
      this._accessToken = accessToken
      this._refreshToken = refreshToken
    }
  },

  clear() {
    this._accessToken = null
    this._refreshToken = null
  },
}

tokenService.setFromStorage()

export default tokenService
