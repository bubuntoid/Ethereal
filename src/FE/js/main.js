const footer = document.querySelector('.footer');
const mainInput = document.querySelector('.main__input-text');
const webPlayer = document.querySelector('.wplayer');
const playerTitle = document.querySelector('.player__title');
const playerSubTitle = document.querySelector('.player__subtitle');
const navList = document.querySelector('.nav-list');
const loader = document.querySelector('.loader');
const downloadBtn = document.querySelector('.download__button');


let footerChecker = (isOpen = false) => {
  footer.style.transition = '1s ease';
  isOpen ? footer.style.transform = 'translate(0, -30%)' : footer.style.transform = 'translate(0, 0%)';
}

function UIChecker(intarface = null,isValid = false) {
  intarface.style.transition = '1s ease'
  if (isValid) {
    intarface.style.opacity = '1' 
    intarface.style.zIndex = '6'
  } else {
    intarface.style.opacity = '0'
    intarface.style.zIndex = '-6'
  }
}

function input2req(url) {
  UIChecker(loader ,true)
  url = mainInput.value
  console.log(url)
  getChapters(url);
}

UIChecker(webPlayer, false)