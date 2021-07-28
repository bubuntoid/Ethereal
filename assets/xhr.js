const reqURL = "http://81.177.135.200:322/api/download/zip"
const xhr = new XMLHttpRequest()

function sendReq(method, url, body = null) {
  return new Promise((resolve, reject) => {
    xhr.open(method, url)
    xhr.responseType = 'json'
    xhr.setRequestHeader('Content-Type', 'application/json')
    xhr.onload = () => {
      xhr.status >= 400 ?  reject('invalid request url') : resolve(xhr.response);
    }
    xhr.send(JSON.stringify(body))
  })
}