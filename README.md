TODO: 
- Use containerized database and add to docker-compose
- Containerize alarm system scripts and add to docker-compose
- ~~use containerized mosquitto and add to docker-compose~~
- ~~create script to easily transfer all secrets to home assistant host~~
- cleanup multi script loops in favor of new repeat mode
- ~~Setup stub secrets for futur travis integration~~
- ~~Setup travis to validate configs in PR~~
- Automatically update config for hass and restart when build pass on master
- Automatically update config for hass and restart when build pass on specific branches/commit/PR?
- ~~Read .HA_VERSION to pull correct image in travis~~
- Allow to build with latest Home assistant version to check config before upgrade Home Assistant
    - Would be nice if HAUpdate.sh:
      1) Stop and error if not on master or pending changes
      2) Pull latest master
      3) Starts a build in travis with latest Home assistant image
      4) Wait and verify for build success
      5) redeploy docker-compose
      6) Wait for complete deployment
      7) commit .HA_VERSION 
      8) push to master (On build success, should not redeploy to Home assistant host.. label? keyword in commit?)
      
      
Helpful links:
To forward github's webhooks to local server without opening inbound port (smee.io):
https://www.jenkins.io/blog/2019/01/07/webhook-firewalls/

To let the smee.io client container call deploy script on host:
https://askubuntu.com/questions/1168090/configure-ssh-server-to-not-ask-key-or-password-over-localhost-connection
