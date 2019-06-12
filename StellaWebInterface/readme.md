# StellaWebInterface

## Description
Gives the ability to interact with stella through a web interface. Hosted on the server and cliented on any device that one user has control over.

## Installation
There are to parts of this application a front-end and a back-end. For both yoiu will need NPM package manager that comes with your node js installation which can be downloaded here: [node.js](https://nodejs.org/en/).

* Edit the ip addresses in 'backend/config/config.json' to your own ip address.
* ### The frontend
    * Have an installed version of [node.js](https://nodejs.org/en/).
    * Then do the following commands in a command tab
      ```bash
      npm i 
      npm start
      ```

* ### The backend
    * Have an installed version of [node.js](https://nodejs.org/en/).
    * Then do the following commands in another tab
      ```bash
      cd /backend
      npm i 
      npm run watcher
      ```