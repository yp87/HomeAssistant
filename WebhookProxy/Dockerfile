FROM node:10-buster-slim

RUN npm install -g smee-client

COPY secret_smeeurl /WebhookProxy/smeeurl
COPY run.sh /WebhookProxy/run.sh

RUN chmod a+x /WebhookProxy/run.sh

CMD [ "/WebhookProxy/run.sh" ]

