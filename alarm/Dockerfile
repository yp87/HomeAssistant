FROM python:3.10.6-slim-buster

ENV LANG C.UTF-8

COPY /Paradox_IP150/requirements.txt /
RUN pip3 install -r requirements.txt

COPY /Paradox_IP150/run.sh /
COPY /Paradox_IP150/ip150.py /
COPY /Paradox_IP150/ip150_mqtt.py /
COPY /secret_options.json /data/options.json

RUN chmod a+x /run.sh

CMD [ "/run.sh" ]
