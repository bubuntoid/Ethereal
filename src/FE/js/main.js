const footer = document.querySelector('.footer');
const mainInput = document.querySelector('.main__input-text');
const webPlayer = document.querySelector('.wplayer');
const playerTitle = document.querySelector('.player__title');
const playerSubTitle = document.querySelector('.player__subtitle');
const navList = document.querySelector('.nav-list');
const navLinks = ''
const loader = document.querySelector('.loader');
const chapters = {};


let footerChecker = (isOpen = false) => {
  footer.style.transition = '1s ease';
  isOpen ? footer.style.transform = 'translate(0, -30vh)' : footer.style.transform = 'translate(0, 0vh)';
}

function loaderChecker(isValid = false) {
  loader.style.transition = '1s ease'
  if (isValid) {
    loader.style.opacity = '1' 
    loader.style.zIndex = '1'
  } else {
    loader.style.opacity = '0'
    loader.style.zIndex = '0'
  }
}

function input2req(url) {
  loaderChecker(true)
  url = mainInput.value
  console.log(url)
  sendReq('POST', reqURL, {
    url: url
  })
}

