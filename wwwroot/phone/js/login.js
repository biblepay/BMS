let lastDataFromLocalStorage;
const lastData = {};
const serverIpInput = document.getElementById('input-server-ip');

document.addEventListener('DOMContentLoaded', () => {
	});


const init = async (username, password) => {
  try {
    await sdk.init({
      localVideoContainerId: 'local_video_holder', // Id of HTMLElement that is used as a default container for local video elements
      serverIp: false, // IP address of a particular media gateway for connection. If it's not specified, IP address will be chosen automatically
      showDebugInfo: false, // Show debug info in the console
    });
  } catch (e) {}
  await connectToVoxCloud(username, password, false);
};

const connectToVoxCloud = async (username, password, connectivityCheck = false) => {
  try {
    await sdk.connect(connectivityCheck);
    localStorage.setItem('lastConnection', JSON.stringify({ connected: true }));
  }
  catch (e) {
    // disable inputs if the server IP is incorrect or if it's impossible to connect to the server with connectivity check on
    console.log('unable to connect to cloud');
    console.log(e);
    return;
  }
  console.log('connected to cloud');
  await signIn(username, password);
};

const signIn = async (username, password) => {
  try {
    await sdk.login(username, password);
    console.log('signed in ');
    document.querySelector('.page_action').classList.remove('hidden');
    const authData = document.querySelector('.action_auth-data');
    authData.querySelector('h2').innerText = `Logged in as ${username}`;
    authData.querySelector('h3').innerText = `You're using Web SDK version ${sdk.version}`;
    console.log('finished sign in');

  } catch (e) {
      console.log(e);
  }
};

